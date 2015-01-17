using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using FizzWare.NBuilder;

using DynamicExpression = System.Linq.Dynamic.DynamicExpression;

namespace RepositoryTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var repository = new PersonRepository();

            // String with param
            const string query = "Age <= 5 AND FavoriteDay ==@0";
            var expression = DynamicExpression.ParseLambda<Person, bool>(query, new DateTime(2015, 1, 20));

            // String without param
            //const query = "Age <= 5";
            //var expression = DynamicExpression.ParseLambda<Person, bool>(query);

            var people = repository.Find(expression).ToList();

            // Using linq
            //var people = repository.Find(p => p.Age <= 5).ToList();

            Console.WriteLine("{0} people are under 5.", people.Count);
            foreach (var p in people)
            {
                Console.WriteLine("{0} is {1} years old and weighs {2}.  Their favorite day is {3}", p.Name, p.Age, p.Weight, p.FavoriteDay.ToShortDateString());
            }

            Console.ReadLine();
        }
    }

    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int Weight { get; set; }
        public DateTime FavoriteDay { get; set; }
    }

    public class PersonContext : DbContext
    {
        public PersonContext()
        {
            Database.SetInitializer(new PersonInitializer());
        }

        public DbSet<Person> People { get; set; }
        
        
    }

    public class PersonInitializer : DropCreateDatabaseAlways<PersonContext>
    {
        protected override void Seed(PersonContext context)
        {
            var people = Builder<Person>.CreateListOfSize(100).Build();
            context.People.AddRange(people);

            base.Seed(context);
        }
    }

    public interface IRepository<T>
    {
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
    }

    public class PersonRepository : IRepository<Person>
    {
        private readonly PersonContext _context;

        public PersonRepository()
        {
            _context = new PersonContext();
        }
        public IEnumerable<Person> Find(Expression<Func<Person, bool>> predicate)
        {
            return _context.People.Where(predicate).ToList();
        }
    }

}
