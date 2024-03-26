using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB_V2.Worker
{
    public class Git
    {
        private string Branch;
        private string Repo;

        public Git(string branch, string repo)
        {
            Branch = branch;
            Repo = repo;
        }
    }
}
