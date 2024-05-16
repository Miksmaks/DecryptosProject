using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticProject3.DownloadData
{
    public struct User
    {
        public int Id;
        public string FirstName;
        public string SecondName;
        public string ThirdName;
        public User(int id, string firstName, string secondName, string thirdName) 
        {
            Id = id;
            FirstName = firstName;
            SecondName = secondName;
            ThirdName = thirdName;
        }
    }
}
