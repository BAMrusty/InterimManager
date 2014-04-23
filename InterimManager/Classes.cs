using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Soap;

namespace InterimManager
{
    abstract class Person
    {
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string telephoneNumber { get; set; }

        public Person(string firstname, string lastname, string telephoneNumber)
        {
            this.firstname = firstname;
            this.lastname = lastname;
            this.telephoneNumber = telephoneNumber;
        }
    }

    class Contact : Person
    {
        public string enterprise { get; set; }

        public Contact(string firstname, string lastname, string telephoneNumber, string enterprise)
            : base(firstname, lastname, telephoneNumber)
        {
            this.enterprise = enterprise;
        }
    }

    class EmployeInterim : Person
    {
        public List<string> skills { get; set; }
        public double fsalary { get; set; }
        public double vsalary { get; set; }
        public List<Mission> missions { get; set; }

        public EmployeInterim(string firstname, string lastname, string telephoneNumber)
            : base(firstname, lastname, telephoneNumber)
        {
            this.fsalary = 0;
            this.vsalary = 0;
            this.skills = new List<string>();
            this.missions = new List<Mission>();
        }
        
        public EmployeInterim(string firstname, string lastname, string telephoneNumber, string fsalary, string vsalary) 
            : base(firstname, lastname, telephoneNumber)
        {
            this.fsalary = Double.Parse(fsalary);
            this.vsalary = Double.Parse(vsalary);
            this.skills = new List<string>();
            this.missions = new List<Mission>();
        }

        public EmployeInterim(string firstname, string lastname, string telephoneNumber, string[] skills)
            : base(firstname, lastname, telephoneNumber)
        {
            this.fsalary = 0;
            this.vsalary = 0;
            this.skills = new List<string>();
            this.missions = new List<Mission>();
            foreach (string item in skills)
            {
                this.skills.Add(item);
            }
        }

        public EmployeInterim(string firstname, string lastname, string telephoneNumber, string fsalary, string vsalary, string [] skills)
            : base(firstname, lastname, telephoneNumber)
        {
            this.fsalary = Double.Parse(fsalary);
            this.vsalary = Double.Parse(vsalary);
            this.skills = new List<string>();
            this.missions = new List<Mission>();
            foreach (string item in skills)
            {
                this.skills.Add(item);
            }
        }

        public void AddSkill(string skill)
        {
            if (!this.skills.Contains(skill))
                this.skills.Add(skill);
        }

        public void RemoveSkill(string skill)
        {
            if (this.skills.Contains(skill))
                this.skills.Remove(skill);
        }

        public void AddSkills(string [] skills)
        {
            foreach (string item in skills)
            {
                this.AddSkill(item);
            }
            this.skills.Remove("");
        }

        public void RemoveSkills(string [] skills)
        {
            foreach (string item in skills)
            {
                this.RemoveSkill(item);
            }
        }

        public string GetSkills()
        {
            string skills = "";
            foreach (string item in this.skills)
            {
                skills += item + " ; ";
            }
            if (skills == "")
                return "None";
            else
                return skills;
        }

        public void AddMission(Mission mission)
        {
            bool test = true;
            foreach (Mission item in this.missions)
            {
                if (!mission.IsCompatible(item))
                    test = false;
            }
            if (test)
                this.missions.Add(mission);
        }

        public string GetMissions()
        {
            string missions = "";
            foreach (Mission item in this.missions)
            {
                missions += item.enterpriseName + " #" + item.id + " ; ";
            }
            if (missions == "")
                return "None";
            else
                return missions;
        }

        public string GetSalary()
        {
            if (this.fsalary == 0 && this.vsalary == 0)
                return "Undefined";
            if (this.vsalary != 0)
                return "Variable";
            else
                return this.fsalary.ToString();
        }

    }

    class Mission
    {
        public string enterpriseName { get; set; }
        public string id { get; set; }
        public DateTime outset { get; set; }
        public DateTime termination { get; set; }
        public bool status { get; set; }
        public List<EmployeInterim> interims { get; set; }

        public Mission(string enterprisename, string id, DateTime outset, DateTime termination)
        {
            this.enterpriseName = enterprisename;
            this.id = id;
            this.outset = outset;
            this.termination = termination;
            this.interims = new List<EmployeInterim>();
            if (this.outset <= DateTime.Now && this.termination >= DateTime.Now)
                this.status = true;
            else
                this.status = false;
            if (this.outset > this.termination)
            {
                DateTime TEMP = this.termination;
                this.termination = this.outset;
                this.outset = TEMP;
            }
        }

        public Mission(string enterprisename, string id, string outset, string termination)
        {
            this.enterpriseName = enterprisename;
            this.id = id;
            this.outset = DateTime.Parse(outset);
            this.termination = DateTime.Parse(termination);
            this.interims = new List<EmployeInterim>();
            if (this.outset <= DateTime.Now && this.termination >= DateTime.Now)
                this.status = true;
            else
                this.status = false;
            if (this.outset > this.termination)
            {
                DateTime TEMP = this.termination;
                this.termination = this.outset;
                this.outset = TEMP;
            }
        }

        public void UpdateStatus()
        {
            if (this.outset <= DateTime.Now && this.termination >= DateTime.Now)
                this.status = true;
            else
                this.status = false;
        }

        public string GetStatus()
        {
            if (this.status)
                return "Active";
            else
                return "Inactive";
        }

        public bool IsCompatible(Mission mission)
        {
            if (this.outset >= mission.outset && this.outset <= mission.termination)
                return false;
            if (this.termination >= mission.outset && this.termination <= mission.termination)
                return false;
            if (this.outset <= mission.outset && this.termination >= mission.termination)
                return false;
            else
                return true;
        }

        public void AddInterim(EmployeInterim interim)
        {
            if (!this.interims.Contains(interim))
                this.interims.Add(interim);
        }

        public string GetInterims()
        {
            string interims = "";
            foreach (EmployeInterim item in this.interims)
            {
                interims += item.firstname + " " + item.lastname + " ; ";
            }
            if (interims == "")
                return "None";
            else
                return interims;
        }

    }

    class Enterprise
    {
        public string name { get; set; }
        public string adress { get; set; }
        public long siret { get; set; }
        public Contact innermate { get; set; }

        public Enterprise(string name, string adress, string siret, Contact innermate)
        {
            this.name = name;
            this.adress = adress;
            this.siret = long.Parse(siret);
            this.innermate = innermate;
        }
    }

    class Serializer
    {
        public static void Serialize(string filename, object o)
        {
            FileStream fs = new FileStream(filename, FileMode.Create);

            SoapFormatter soapf = new SoapFormatter();

            soapf.Serialize(fs, o);

            fs.Close();
        }
    }

    class Deserializer
    {
        public static object Deserialize(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open);

            SoapFormatter soapf = new SoapFormatter();

            object o = soapf.Deserialize(fs);

            fs.Close();

            return o;
        }
    }
}
