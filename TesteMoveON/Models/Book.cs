using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TesteMoveON.Models;

public class Book
{
    public int Id { get; set; }
    
    [Required]
    public string? Title { get; set; }
    
    [Required]
    public string? Author { get; set; }
    
    [Required]
    public string? Genre { get; set; }
}