﻿using Newtonsoft.Json;
using RecipeLewis.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeLewis.Models;

public class DocumentModel : EntityDataUserModel
{
    public int DocumentId { get; set; }
    public string? Name { get; set; }
    public string? FileName { get; set; }
    public string? Description { get; set; }
    public string? ContentType { get; set; }
    public long ByteSize { get; set; }
    public byte[] Bytes { get; set; }
    public Guid DocumentKey { get; set; }
    [JsonIgnore]
    public bool NotUploaded { get; set; }
    [JsonIgnore]
    public string ImgSrc { get; set; }
}

