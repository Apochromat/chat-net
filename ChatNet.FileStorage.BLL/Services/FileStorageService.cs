using System.Text;
using System.Text.RegularExpressions;
using ChatNet.Common.DataTransferObjects;
using ChatNet.Common.Enumerations;
using ChatNet.Common.Exceptions;
using ChatNet.Common.Interfaces;
using ChatNet.FileStorage.DAL;
using ChatNet.FileStorage.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ChatNet.FileStorage.BLL.Services;

/// <summary>
/// Service for file storage
/// </summary>
public class FileStorageService : IFileStorageService {
    private readonly FileStorageDbContext _dbContext;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="configuration"></param>
    public FileStorageService(FileStorageDbContext dbContext, IConfiguration configuration) {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    /// <inheritdoc/>
    public async Task<Guid> UploadFileAsync(FileUploadDto fileUploadDto) {
        using var memoryStream = new MemoryStream();
        await fileUploadDto.Content.CopyToAsync(memoryStream);

        if (memoryStream.Length < 1048576 * _configuration.GetSection("FileStorage").GetValue<int>("MaxFileSizeInMB")) {
            var id = Guid.NewGuid();

            var file = new StoredFile() {
                Id = id,
                Content = memoryStream.ToArray(),
                ContentType = fileUploadDto.ContentType,
                Type = fileUploadDto.Type,
                Name = Transliterate(fileUploadDto.Name),
                IsPublic = fileUploadDto.IsPublic,
                OwnerId = fileUploadDto.OwnerId,
                Viewers = fileUploadDto.Viewers
            };

            await _dbContext.Files.AddAsync(file);
            await _dbContext.SaveChangesAsync();

            return id;
        }
        else {
            throw new ArgumentException("File is too big");
        }
    }

    /// <inheritdoc/>
    public async Task<Pagination<FileInfoDto>> GetOwnedFilesInfoAsync(Guid ownerId, int page, int pageSize,
        List<FileType>? fileTypes = null) {
        if (page < 1) {
            throw new ArgumentException("Page must be greater than 0");
        }

        if (pageSize < 1) {
            throw new ArgumentException("Page size must be greater than 0");
        }

        var files = await _dbContext.Files
            .Where(f => f.OwnerId == ownerId
                        && f.DeletedAt == null
                        && (fileTypes == null || fileTypes.Count == 0 || fileTypes.Contains(f.Type)))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new FileInfoDto() {
                Id = x.Id,
                Name = x.Name,
                Size = new FileSizeDto(x.Content.Length),
                Type = x.Type,
                OwnerId = x.OwnerId,
                Viewers = x.Viewers,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync();

        var pagesAmount = (int)Math.Ceiling((double)files.Count / pageSize);

        if (page > pagesAmount) {
            throw new NotFoundException("Page not found");
        }

        return new Pagination<FileInfoDto>(files, page, pageSize, pagesAmount);
    }

    /// <inheritdoc/>
    public async Task<Pagination<FileInfoDto>> GetSharedWithUserFilesInfoAsync(Guid userId, int page, int pageSize,
        List<FileType>? fileTypes = null) {
        if (page < 1) {
            throw new ArgumentException("Page must be greater than 0");
        }

        if (pageSize < 1) {
            throw new ArgumentException("Page size must be greater than 0");
        }

        var files = await _dbContext.Files
            .Where(f => f.Viewers.Contains(userId)
                        && f.DeletedAt == null
                        && (fileTypes == null || fileTypes.Count == 0 || fileTypes.Contains(f.Type)))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new FileInfoDto() {
                Id = x.Id,
                Name = x.Name,
                Size = new FileSizeDto(x.Content.Length),
                Type = x.Type,
                OwnerId = x.OwnerId,
                Viewers = x.Viewers,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync();

        var pagesAmount = (int)Math.Ceiling((double)files.Count / pageSize);

        if (page > pagesAmount) {
            throw new NotFoundException("Page not found");
        }

        return new Pagination<FileInfoDto>(files, page, pageSize, pagesAmount);
    }

    /// <inheritdoc/>
    public async Task<FileInfoDto> GetFileInfoAsync(Guid fileId, Guid ownerId) {
        var file = await _dbContext.Files.FindAsync(fileId);
        if (file == null) {
            throw new NotFoundException("File not found");
        }

        if (file.DeletedAt != null) {
            throw new NotFoundException("File not found");
        }

        if (file.OwnerId != ownerId && !file.Viewers.Contains(ownerId) && !file.IsPublic) {
            throw new ForbiddenException("You don't have access to this file");
        }

        return new FileInfoDto() {
            Id = file.Id,
            Name = file.Name,
            Size = new FileSizeDto(file.Content.Length),
            Type = file.Type,
            OwnerId = file.OwnerId,
            Viewers = file.Viewers
        };
    }

    /// <inheritdoc/>
    public async Task<FileDownloadDto> DownloadFileAsync(Guid fileId, Guid ownerId) {
        var file = await _dbContext.Files.FindAsync(fileId);
        if (file == null) {
            throw new NotFoundException("File not found");
        }

        if (file.DeletedAt != null) {
            throw new NotFoundException("File not found");
        }

        if (file.OwnerId != ownerId && !file.Viewers.Contains(ownerId) && !file.IsPublic) {
            throw new ForbiddenException("You don't have access to this file");
        }

        return new FileDownloadDto() {
            Id = file.Id,
            Content = file.Content,
            ContentType = file.ContentType,
            Name = file.Name
        };
    }

    /// <inheritdoc/>
    public async Task EditFileAsync(Guid fileId, Guid ownerId, FileEditDto fileEditDto) {
        var file = await _dbContext.Files.FindAsync(fileId);
        if (file == null) {
            throw new NotFoundException("File not found");
        }

        if (file.DeletedAt != null) {
            throw new NotFoundException("File not found");
        }

        if (file.OwnerId != ownerId) {
            throw new ForbiddenException("File can be edited only by its owner");
        }

        file.Name = fileEditDto.Name;
        file.Type = fileEditDto.Type;

        await _dbContext.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task DeleteFileAsync(Guid fileId, Guid ownerId) {
        var file = await _dbContext.Files.FindAsync(fileId);
        if (file == null) {
            throw new NotFoundException("File not found");
        }

        if (file.OwnerId != ownerId) {
            throw new ForbiddenException("File can be deleted only by its owner");
        }

        file.DeletedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task<bool> IsFileOwnerAsync(Guid fileId, Guid userId) {
        var file = await _dbContext.Files.FindAsync(fileId);
        if (file == null) {
            throw new NotFoundException("File not found");
        }

        return file.OwnerId == userId;
    }

    /// <inheritdoc/>
    public async Task<bool> IsFileOwnerOrViewerAsync(Guid fileId, Guid userId) {
        var file = await _dbContext.Files.FindAsync(fileId);
        if (file == null) {
            throw new NotFoundException("File not found");
        }

        return file.OwnerId == userId || file.Viewers.Contains(userId);
    }

    /// <inheritdoc/>
    public async Task AddViewerToFileAsync(Guid fileId, Guid userId, Guid ownerId,
        bool raiseExceptionIfAlreadyExists = false) {
        var file = await _dbContext.Files.FindAsync(fileId);
        if (file == null) {
            throw new NotFoundException("File not found");
        }

        if (file.DeletedAt != null) {
            throw new NotFoundException("File not found");
        }

        if (file.OwnerId != ownerId) {
            throw new ForbiddenException("File can be edited only by its owner");
        }

        if (file.Viewers.Contains(userId)) {
            if (raiseExceptionIfAlreadyExists) {
                throw new ConflictException("User is already a viewer of this file");
            }

            return;
        }

        file.Viewers.Add(userId);
        await _dbContext.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task RemoveViewerFromFileAsync(Guid fileId, Guid userId, Guid ownerId,
        bool raiseExceptionIfNotExists = false) {
        var file = await _dbContext.Files.FindAsync(fileId);
        if (file == null) {
            throw new NotFoundException("File not found");
        }

        if (file.DeletedAt != null) {
            throw new NotFoundException("File not found");
        }

        if (file.OwnerId != ownerId) {
            throw new ForbiddenException("File can be edited only by its owner");
        }

        if (!file.Viewers.Contains(userId)) {
            if (raiseExceptionIfNotExists) {
                throw new ConflictException("User is already a viewer of this file");
            }

            return;
        }

        file.Viewers.Remove(userId);
        await _dbContext.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task AddViewerToFilesAsync(List<Guid> filesId, Guid userId, Guid ownerId,
        bool raiseExceptionIfAlreadyExists = false) {
        var files = await _dbContext.Files.Where(f => filesId.Contains(f.Id)).ToListAsync();
        if (files.Count != filesId.Count) {
            throw new NotFoundException("Some files were not found");
        }

        if (files.Any(x => x.DeletedAt != null)) {
            throw new NotFoundException("Some files were not found");
        }

        if (files.Any(x => x.OwnerId != ownerId)) {
            throw new ForbiddenException("Some files can be edited only by its owner");
        }

        foreach (var file in files) {
            if (file.Viewers.Contains(userId)) {
                if (raiseExceptionIfAlreadyExists) {
                    throw new ConflictException("User is already a viewer of this file");
                }

                continue;
            }

            file.Viewers.Add(userId);
        }
    }

    /// <inheritdoc/>
    public async Task RemoveViewerFromFilesAsync(List<Guid> filesId, Guid userId, Guid ownerId,
        bool raiseExceptionIfNotExists = false) {
        var files = await _dbContext.Files.Where(f => filesId.Contains(f.Id)).ToListAsync();
        if (files.Count != filesId.Count) {
            throw new NotFoundException("Some files were not found");
        }

        if (files.Any(x => x.DeletedAt != null)) {
            throw new NotFoundException("Some files were not found");
        }

        if (files.Any(x => x.OwnerId != ownerId)) {
            throw new ForbiddenException("Some files can be edited only by its owner");
        }

        foreach (var file in files) {
            if (!file.Viewers.Contains(userId)) {
                if (raiseExceptionIfNotExists) {
                    throw new ConflictException("User is already a viewer of this file");
                }

                continue;
            }

            file.Viewers.Remove(userId);
        }
    }

    /// <inheritdoc/>
    public async Task AddViewersToFileAsync(Guid fileId, List<Guid> usersId, Guid ownerId,
        bool raiseExceptionIfAlreadyExists = false) {
        var file = await _dbContext.Files.FindAsync(fileId);
        if (file == null) {
            throw new NotFoundException("File not found");
        }

        if (file.DeletedAt != null) {
            throw new NotFoundException("File not found");
        }

        if (file.OwnerId != ownerId) {
            throw new ForbiddenException("File can be edited only by its owner");
        }

        foreach (var userId in usersId) {
            if (file.Viewers.Contains(userId)) {
                if (raiseExceptionIfAlreadyExists) {
                    throw new ConflictException("User is already a viewer of this file");
                }

                continue;
            }

            file.Viewers.Add(userId);
        }
    }

    /// <inheritdoc/>
    public async Task RemoveViewersFromFileAsync(Guid fileId, List<Guid> usersId, Guid ownerId,
        bool raiseExceptionIfNotExists = false) {
        var file = await _dbContext.Files.FindAsync(fileId);
        if (file == null) {
            throw new NotFoundException("File not found");
        }

        if (file.DeletedAt != null) {
            throw new NotFoundException("File not found");
        }

        if (file.OwnerId != ownerId) {
            throw new ForbiddenException("File can be edited only by its owner");
        }

        foreach (var userId in usersId) {
            if (!file.Viewers.Contains(userId)) {
                if (raiseExceptionIfNotExists) {
                    throw new ConflictException("User is already a viewer of this file");
                }

                continue;
            }

            file.Viewers.Remove(userId);
        }
    }

    private string Transliterate(string input) {
        var transliterationMap = new Dictionary<char, string> {
            { 'а', "a" },
            { 'б', "b" },
            { 'в', "v" },
            { 'г', "g" },
            { 'д', "d" },
            { 'е', "e" },
            { 'ё', "yo" },
            { 'ж', "zh" },
            { 'з', "z" },
            { 'и', "i" },
            { 'й', "y" },
            { 'к', "k" },
            { 'л', "l" },
            { 'м', "m" },
            { 'н', "n" },
            { 'о', "o" },
            { 'п', "p" },
            { 'р', "r" },
            { 'с', "s" },
            { 'т', "t" },
            { 'у', "u" },
            { 'ф', "f" },
            { 'х', "kh" },
            { 'ц', "ts" },
            { 'ч', "ch" },
            { 'ш', "sh" },
            { 'щ', "sch" },
            { 'ъ', "" },
            { 'ы', "y" },
            { 'ь', "" },
            { 'э', "e" },
            { 'ю', "yu" },
            { 'я', "ya" },
            { ' ', "_" }
        };

        string fileName = Path.GetFileNameWithoutExtension(input);
        string extension = Path.GetExtension(input);

        var transliterated = new StringBuilder();
        foreach (var character in fileName.ToLower()) {
            if (transliterationMap.TryGetValue(character, out var value)) {
                transliterated.Append(value);
            }
            else {
                transliterated.Append(character);
            }
        }

        var regex = new Regex("[^a-zA-Z0-9_]");
        var cleaned = regex.Replace(transliterated.ToString(), "");

        return cleaned + extension;
    }
}