using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using GymManagementDAL.Data.Repositories.Interfaces;
using GymManagementDAL.Entities;

namespace GymManagementBLL.Services.Specifications
{
    public class TrainerWithFilterSpecification : Specification<Trainer>
    {
        public TrainerWithFilterSpecification(string email, string phone, int? id) : base(x => (x.Email == email || x.Phone == phone) && x.Id != id) { }
    }
}
