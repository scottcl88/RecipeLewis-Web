using RecipeLewis.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeLewis.Models;
public class TagModel : EntityDataUserModel
{
    public TagModel() { }
    public TagModel(string name)
    {
        Name = name;
    }
    public int TagId { get; set; }
    public string Name { get; set; }
    public string Alias { get; set; }
}