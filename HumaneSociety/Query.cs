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
            catch(InvalidOperationException e)
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
            if(updatedAddress == null)
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
            throw new NotImplementedException();
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
            Animal animalToUpdate = null;
            try
            {
                animalToUpdate = db.Animals.Where(c => c.AnimalId == animalId).FirstOrDefault();
            }
            catch
            {
                Console.WriteLine("The id given doesn't match one in the system");
                Console.WriteLine("No changes have been saved");
                Console.WriteLine("Please try again");
            }
            foreach(KeyValuePair<int,string> value in updates)
            {
                switch (value.Key)
                {
                    case 1:
                        animalToUpdate.Age = int.Parse(value.Value);
                        break;
                    case 2:
                        animalToUpdate.Category.Name = value.Value;
                        break;
                    case 3:
                        animalToUpdate.Demeanor = value.Value;
                        break;
                    case 4:
                        animalToUpdate.DietPlan.Name  = value.Value;
                        break;
                    case 5:
                        animalToUpdate.Gender = value.Value;
                        break;
                    case 6:
                        animalToUpdate.KidFriendly = Convert.ToBoolean(value.Value);
                        break;
                    case 7:
                        animalToUpdate.Name = value.Value;
                        break;
                    case 8:
                        animalToUpdate.PetFriendly = Convert.ToBoolean(value.Value);
                        break;
                    case 9:
                        animalToUpdate.Weight = int.Parse(value.Value);
                        break;
                    case 0:
                        animalToUpdate.AdoptionStatus = value.Value;
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
        internal static List<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            List<Animal> animalsToSeach = db.Animals.ToList();
            foreach (KeyValuePair<int,string> update in updates)
            {
                switch (update.Key)
                {
                    case 1:
                        animalsToSeach = animalsToSeach.Where(i => i.Age == Convert.ToInt32(update.Value)).ToList();
                        break;
                    case 2:
                        animalsToSeach = animalsToSeach.Where(j => j.AnimalId == Convert.ToInt32(update.Value)).ToList();
                        break;
                    case 3:
                        animalsToSeach = animalsToSeach.Where(k => k.CategoryId == Convert.ToInt32(update.Key)).ToList();
                        break;
                    case 4:
                        animalsToSeach = animalsToSeach.Where(l => l.Demeanor == (update.Value)).ToList();
                        break;
                    case 5:
                        animalsToSeach = animalsToSeach.Where(m => m.DietPlanId == Convert.ToInt32(update.Key)).ToList();
                        break;
                    case 6:
                        animalsToSeach = animalsToSeach.Where(n => n.Gender == (update.Value)).ToList();
                        break;
                    case 7:
                        animalsToSeach = animalsToSeach.Where(o => o.KidFriendly == Convert.ToBoolean(update.Value)).ToList();
                        break;
                    case 8:
                        animalsToSeach = animalsToSeach.Where(p => p.Name == (update.Value)).ToList();
                        break;
                    case 9:
                        animalsToSeach = animalsToSeach.Where(q => q.PetFriendly == Convert.ToBoolean(update.Value)).ToList();
                        break;
                    case 0:
                        animalsToSeach = animalsToSeach.Where(r => r.Weight == Convert.ToInt32(update.Value)).ToList();
                        break;
                    default:
                        break;
                }
            }
            return animalsToSeach;
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
            return db.Adoptions;
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            Adoption adoptionTobeUpdated = null;
            adoptionTobeUpdated = db.Adoptions.Where(g => g.AnimalId == adoption.AnimalId && g.ClientId == adoption.ClientId).SingleOrDefault();
            adoptionTobeUpdated.ApprovalStatus = isAdopted.ToString();
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            Adoption removeAdoption = null;
            removeAdoption = db.Adoptions.Where(h => h.AnimalId == animalId && h.ClientId == clientId).SingleOrDefault();
            db.Adoptions.DeleteOnSubmit(removeAdoption);
            db.SubmitChanges();
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            throw new NotImplementedException();
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            throw new NotImplementedException();
        }
    }
}