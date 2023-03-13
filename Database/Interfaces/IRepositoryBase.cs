using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Database.Interfaces
{

    public class Repository<Entity, TKey>
        where Entity : EntityBase<TKey>
        where TKey : IEquatable<TKey>
    {
       
    }
}
