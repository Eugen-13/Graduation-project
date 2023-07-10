using GameClub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClub.Repositories
{
    internal interface ISelectable<T> where T : class
    {
        public List<T> SelectAll();
        public List<T> SelectAllByOption(string findText, string findField, bool isAdminSelect);

    }
}
