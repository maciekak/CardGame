using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.DatabaseAccessLayer.Model
{
    public class Player
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int RankingPoints { get; set; }
    }
}