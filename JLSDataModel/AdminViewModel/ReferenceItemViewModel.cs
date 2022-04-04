﻿using JLSDataModel.Models;
using System.Collections.Generic;

namespace JLSDataModel.ViewModels
{
    public class ReferenceItemViewModel
    {
        public ReferenceItemViewModel()
        {
            this.Labels = new List<ReferenceLabel>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public long? ParentId { get; set; }
        public string Value { get; set; }
        public int? Order { get; set; }
        public string Category { get; set; }
        public long ReferenceCategoryId { get; set; }
        public List<ReferenceLabel> Labels { get; set; }
        public string Label { get; set; }
        public string Lang { get; set; }
        public bool? Validity { get; set; }
    }
}