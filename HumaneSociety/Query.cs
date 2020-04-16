using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {
        static HumaneSociety2DataContext db;

        static Query()
        {
            db = new HumaneSociety2DataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();

            return allStates;
        }

        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = null;

            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("No clients have a ClientId that matches the Client passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }

            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;

            // submit changes
            db.SubmitChanges();
        }

        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName != null;
        }

        //// TODO Items: ////

        // TODO: Allow any of the CRUD operations to occur here
        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {
            switch (crudOperation)
            {
                case "create":
                    CreateNewEmployee();
                    break;
                case "read":
                    Console.WriteLine(employee.FirstName, employee.LastName, employee.UserName, employee.Email);
                    db.SubmitChanges();
                    break;
                case "Update":
                    UpdateEmployee(employee);
                    break;
                case "Delete":
                    db.Employees.DeleteOnSubmit(employee);
                    db.SubmitChanges();
                    break;
                default:
                    Console.WriteLine();
                    break;
            }

        }
        internal static void CreateNewEmployee()
        {
            Employee employee = new Employee();
            employee.FirstName = UserInterface.GetStringData("first name", "the employee's");
            employee.LastName = UserInterface.GetStringData("last name", "the employee's");
            employee.EmployeeNumber = int.Parse(UserInterface.GetStringData("employee number", "the employee's"));
            employee.Email = UserInterface.GetStringData("email", "the employee's");
            db.Employees.InsertOnSubmit(employee);
            db.SubmitChanges();

        }

        internal static void UpdateEmployee(Employee employee)
        {

            Employee employee1 = new Employee(); //////newEmployee needs to be fixed/////
            employee.FirstName = UserInterface.GetStringData("first name", "the employee's");
            employee.LastName = UserInterface.GetStringData("last name", "the employee's");
            employee.EmployeeNumber = int.Parse(UserInterface.GetStringData("employee number", "the employee's"));
            employee.Email = UserInterface.GetStringData("email", "the employee's");
            try
            {
                Query.RunEmployeeQueries(employee, "update");
                UserInterface.DisplayUserOptions("Employee update successful.");
                //
            }
            catch
            {

                Console.Clear();
                UserInterface.DisplayUserOptions("Employee update unsuccessful please try again or type exit;");
                return;

            }


        }

        // TODO: Animal CRUD Operations
        internal static void AddAnimal(Animal animal)
        {
            var animalCategoryName = UserInterface.GetStringData("Category", "The Name of the Animal is");
            var animalDietPlanName = UserInterface.GetStringData("Diet Plan ", "The Name of the Animal's");

            animal.CategoryId = Query.GetCategoryId(animalCategoryName);
            animal.AdoptionStatus = UserInterface.GetStringData("Adoption Status", "The Animal");
            animal.Age = UserInterface.GetIntegerData("The Animal's", "Age");
            animal.AnimalId = UserInterface.GetIntegerData("The Animal's", "ID Number");
            animal.Demeanor = UserInterface.GetStringData("Demeanor", "The Animal's");
            animal.DietPlanId = Query.GetDietPlanId(animalDietPlanName);
            animal.Gender = UserInterface.GetStringData("Gender", "The Animal is");
            animal.KidFriendly = UserInterface.GetBitData("Kidfriendly", "The Animal is");
            animal.Name = UserInterface.GetStringData("Name", "The Animal's");
            animal.PetFriendly = UserInterface.GetBitData("Petfriendly", "The Animal is");
            animal.Weight = UserInterface.GetIntegerData("The Animal's", "Weight");
        }

        internal static Animal GetAnimalByID(int id)
        {
            return db.Animals.FirstOrDefault(b => b.AnimalId == id);
        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {
            Animal animalToBeUpdated = null;
            try
            {
                animalToBeUpdated = db.Animals.Where(g => g.AnimalId == animalId).FirstOrDefault();
            }
            catch
            {
                Console.WriteLine("No Id was found matching");
                Console.WriteLine("Please retry and enter a vaild Id");
                return;
            }
            foreach (KeyValuePair<int,string> value in updates)
            {
                switch (value.Key)
                {
                    case 1:
                        animalToBeUpdated.AdoptionStatus = value.Value;
                        break;
                    case 2:
                        animalToBeUpdated.Age = int.Parse(value.Value);
                        break;
                    case 3:
                        animalToBeUpdated.AnimalId = int.Parse(value.Value);
                        break;
                    case 4:
                        animalToBeUpdated.Category.Name = value.Value;
                        break;
                    case 5:
                        animalToBeUpdated.Demeanor = value.Value;
                        break;
                    case 6:
                        animalToBeUpdated.DietPlan.Name = value.Value;
                        break;
                    case 7:
                        animalToBeUpdated.Gender = value.Value;
                        break;
                    case 8:
                        animalToBeUpdated.KidFriendly = Convert.ToBoolean(value.Value);
                        break;
                    case 9:
                        animalToBeUpdated.Name = value.Value;
                        break;
                    case 0:
                        animalToBeUpdated.PetFriendly = Convert.ToBoolean(value.Value);
                        break;
                    default:
                        break;
                }
            }
            db.SubmitChanges();
        }

        internal static void RemoveAnimal(Animal animal)
        {
            db.Animals.DeleteOnSubmit(animal);
            db.SubmitChanges();
        }

        // TODO: Animal Multi-Trait Search
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            throw new NotImplementedException();
        }

        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName)
        {
            var categoryIDWanted = db.Categories.Where(c => c.Name == categoryName).Select(d => d.CategoryId).FirstOrDefault();
            return categoryIDWanted;
        }

        internal static Room GetRoom(int animalId)
        {
            var rooms = db.Rooms.Where(e => e.AnimalId == animalId).SingleOrDefault();
            return rooms;
        }

        internal static int GetDietPlanId(string dietPlanName)
        {
            var dietPlanIdWanted = db.DietPlans.Where(f => f.Name == dietPlanName).Select(g => g.DietPlanId).FirstOrDefault();
            return dietPlanIdWanted;
        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {
                Adoption newAdoption = new Adoption();
                newAdoption.AdoptionFee = 75;
                newAdoption.AnimalId = animal.AnimalId;
                newAdoption.ClientId = client.ClientId;
                newAdoption.ApprovalStatus = "Approved";
                newAdoption.PaymentCollected = true;
                db.Adoptions.InsertOnSubmit(newAdoption);
                db.SubmitChanges();
            
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            throw new NotImplementedException();
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            throw new NotImplementedException();
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            throw new NotImplementedException();
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            var shots = db.AnimalShots.Where(a => a.AnimalId == animal.AnimalId);
            return db.AnimalShots;
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            Shot newShot = new Shot();
            shotName = newShot.Name;
            db.Shots.InsertOnSubmit(newShot);
            db.SubmitChanges();
            AnimalShot ShotForAnimal = new AnimalShot();
            ShotForAnimal.AnimalId = animal.AnimalId;
            db.AnimalShots.InsertOnSubmit(ShotForAnimal);
            db.SubmitChanges();


        }
    }
}   
