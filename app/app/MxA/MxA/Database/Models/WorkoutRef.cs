﻿using SQLite;

namespace MxA.Database.Models {
   public class WorkoutRef : IModel {
      [PrimaryKey]
      public string Id { get; set; }
      public string Label { get; set; }
      public string WorkoutId { get; set; }
      public string ProgressionId { get; set; }
      public uint Order { get; set; }
   }
}
