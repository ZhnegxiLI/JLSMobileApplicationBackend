using System;
using System.Collections.Generic;
using System.Text;

namespace JLSDataModel.Models.User
{
    public class UserCountInfo:BaseObject
    {
        public int OrderCount { get; set; }

        public int CommentCount { get; set; }

        public int FavoriteCount { get; set; }
    }
}
