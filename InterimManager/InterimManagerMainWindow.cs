using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace InterimManager
{
    public partial class InterimManagerMainWindow : Form
    {
        private List<EmployeInterim> ListInterimsInit;
        private List<Enterprise> ListEnterprisesInit;
        private List<Mission> ListMissionsInit;
        private List<EmployeInterim> ListInterimsTemp;
        private List<Enterprise> ListEnterprisesTemp;
        private List<Mission> ListMissionsTemp;


        public InterimManagerMainWindow()
        {
            InitializeComponent();
            this.ListEnterprisesInit = new List<Enterprise>();
            this.ListInterimsInit = new List<EmployeInterim>();
            this.ListMissionsInit = new List<Mission>();
            this.ListEnterprisesTemp = new List<Enterprise>();
            this.ListInterimsTemp = new List<EmployeInterim>();
            this.ListMissionsTemp = new List<Mission>();
        }

        private void toolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void toolStripMenuItemLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog FileDialog = new OpenFileDialog();
            FileDialog.Filter = "XML files | *.xml"; 
            FileDialog.Multiselect = false; 
            if (FileDialog.ShowDialog() == DialogResult.OK) 
            {
                String path = FileDialog.FileName; 
                XDocument doc = XDocument.Load(path);

                this.ListEnterprisesInit.Clear();
                this.ListInterimsInit.Clear();
                this.ListMissionsInit.Clear();
                this.ListEnterprisesTemp.Clear();
                this.ListInterimsTemp.Clear();
                this.ListMissionsTemp.Clear();


                var interims = from interim in doc.Descendants("interim")
                            select new
                            {
                                Firstname = interim.Element("firstname").Value,
                                Lastname = interim.Element("lastname").Value,
                                Phone = interim.Element("phone").Value,
                                Fsalary = interim.Element("fsalary").Value,
                                Vsalary = interim.Element("vsalary").Value
                            };

                var skills = from skill in doc.Descendants("skills")
                             select new
                             {
                                 Skills = skill.Value
                             };

                var enterprises = from enterprise in doc.Descendants("enterprise")
                                  select new
                                  {
                                      Name = enterprise.Element("name").Value,
                                      Adress = enterprise.Element("adress").Value,
                                      Siret = enterprise.Element("siret").Value,
                                      ContactFirstname = enterprise.Element("contact").Element("firstname").Value,
                                      ContactLastname = enterprise.Element("contact").Element("lastname").Value,
                                      ContactPhone = enterprise.Element("contact").Element("phone").Value
                                  };

                var missions = from mission in doc.Descendants("mission")
                                  select new
                                  {
                                      Name = mission.Element("enterprisename").Value,
                                      Id = mission.Element("id").Value,
                                      Outset = mission.Element("outset").Value,
                                      Termination = mission.Element("termination").Value,
                                  };

                foreach (var mission in missions)
                {
                    this.ListMissionsInit.Add(new Mission(mission.Name, mission.Id, mission.Outset, mission.Termination));
                    this.ListMissionsTemp.Add(new Mission(mission.Name, mission.Id, mission.Outset, mission.Termination));
                }

                foreach (var enterprise in enterprises)
                {
                    this.ListEnterprisesInit.Add(new Enterprise(enterprise.Name, enterprise.Adress, enterprise.Siret, new Contact(enterprise.ContactFirstname, enterprise.ContactLastname, enterprise.ContactPhone, enterprise.Name)));
                    this.ListEnterprisesTemp.Add(new Enterprise(enterprise.Name, enterprise.Adress, enterprise.Siret, new Contact(enterprise.ContactFirstname, enterprise.ContactLastname, enterprise.ContactPhone, enterprise.Name)));
                }

                foreach (var interim in interims)
                {

                    this.ListInterimsInit.Add(new EmployeInterim(interim.Firstname, interim.Lastname, interim.Phone, interim.Fsalary, interim.Vsalary));
                    this.ListInterimsTemp.Add(new EmployeInterim(interim.Firstname, interim.Lastname, interim.Phone, interim.Fsalary, interim.Vsalary));
                }

                List<string> unsplitSkills = new List<string>();
                List<string[]> splitSkills = new List<string[]>();

                foreach (var skill in skills)
                {
                    unsplitSkills.Add(skill.Skills);
                }
                foreach (string item in unsplitSkills)
                {
                    splitSkills.Add(item.Split(';'));
                }

                for (int i = 0; i < this.ListInterimsInit.Count; i++)
                {
                    this.ListInterimsInit[i].AddSkills(splitSkills[i]);
                    this.ListInterimsTemp[i].AddSkills(splitSkills[i]);
                }

                DisplayInterim();
                DisplayEnterprise();
                DisplayMission();

                this.contextMenuStripListboxInterim.Enabled = true;
                this.deleteToolStripMenuItem.Enabled = true;
                this.buttonClearInterim.Enabled = true;
                this.buttonSearchInterim.Enabled = true;
                this.comboBoxSearchInterim.Enabled = true;
                this.textBoxSearchInterim.Enabled = true;
                this.buttonClearEnterprise.Enabled = true;
                this.buttonSearchEnterprise.Enabled = true;
                this.comboBoxSearchEnterprise.Enabled = true;
                this.textBoxSearchEnterprise.Enabled = true;
                this.comboBoxSearchMission.Enabled = true;
                this.textBoxSearchMission.Enabled = true;
                this.buttonSearchMission.Enabled = true;
                this.buttonClearMission.Enabled = true;
                this.buttonAddInterim.Enabled = true;
                this.toolStripMenuItemSave.Enabled = true;
            }
        }

        private void DisplayInterim()
        {
            this.listBoxInterimFirstname.Items.Clear();
            this.listBoxInterimLastname.Items.Clear();
            this.listBoxInterimMissions.Items.Clear();
            this.listBoxInterimPhone.Items.Clear();
            this.listBoxInterimSkills.Items.Clear();
            this.listBoxInterimSalary.Items.Clear();

            foreach (EmployeInterim item in this.ListInterimsTemp)
            {
                this.listBoxInterimFirstname.Items.Add(item.firstname);
                this.listBoxInterimLastname.Items.Add(item.lastname);
                this.listBoxInterimPhone.Items.Add(item.telephoneNumber);
                this.listBoxInterimSkills.Items.Add(item.GetSkills());
                this.listBoxInterimMissions.Items.Add(item.GetMissions());
                this.listBoxInterimSalary.Items.Add(item.GetSalary());
            }
        }

        private void DisplayEnterprise()
        {
            this.listBoxEnterpriseName.Items.Clear();
            this.listBoxEnterpriseAdress.Items.Clear();
            this.listBoxEnterpriseSiret.Items.Clear();
            this.listBoxContactFirstname.Items.Clear();
            this.listBoxContactLastname.Items.Clear();
            this.listBoxContactPhone.Items.Clear();

            foreach (Enterprise item in this.ListEnterprisesTemp)
            {
                this.listBoxEnterpriseName.Items.Add(item.name);
                this.listBoxEnterpriseAdress.Items.Add(item.adress);
                this.listBoxEnterpriseSiret.Items.Add(item.siret);
                this.listBoxContactFirstname.Items.Add(item.innermate.firstname);
                this.listBoxContactLastname.Items.Add(item.innermate.lastname);
                this.listBoxContactPhone.Items.Add(item.innermate.telephoneNumber);
            }
        }

        private void DisplayMission()
        {
            this.listBoxMissionEnterprise.Items.Clear();
            this.listBoxMissionId.Items.Clear();
            this.listBoxMissionInterims.Items.Clear();
            this.listBoxMissionOutset.Items.Clear();
            this.listBoxMissionStatus.Items.Clear();
            this.listBoxMissionTermination.Items.Clear();

            foreach (Mission item in this.ListMissionsTemp)
            {
                this.listBoxMissionEnterprise.Items.Add(item.enterpriseName);
                this.listBoxMissionId.Items.Add(item.id);
                this.listBoxMissionInterims.Items.Add(item.GetInterims());
                this.listBoxMissionOutset.Items.Add(item.outset.ToString());
                this.listBoxMissionStatus.Items.Add(item.GetStatus());
                this.listBoxMissionTermination.Items.Add(item.termination.ToString());
            }
        }

        private void GettingBoredToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("( (╯°□°）╯︵ ┻━┻ )");
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int index = listBoxInterimFirstname.SelectedIndex;
            string temp = listBoxInterimFirstname.SelectedItem.ToString();
            listBoxInterimLastname.SetSelected(index, true);
            listBoxInterimPhone.SetSelected(index, true);
            temp += " " + listBoxInterimLastname.SelectedItem.ToString();
            foreach (EmployeInterim item in ListInterimsTemp)
            {
                if ((item.firstname == listBoxInterimFirstname.SelectedItem.ToString()) && (item.lastname == listBoxInterimLastname.SelectedItem.ToString()) && (item.telephoneNumber == listBoxInterimPhone.SelectedItem.ToString()))
                {
                    ListInterimsTemp.Remove(item);
                    break;
                }
            }

            DisplayInterim();

            System.Windows.Forms.MessageBox.Show(temp + " has been deleted.");
        }

        private void buttonClearInterim_Click(object sender, EventArgs e)
        {
            DisplayInterim();
        }

        private void buttonSearchInterim_Click(object sender, EventArgs e)
        {
            bool test = false;
            if (this.textBoxSearchInterim.TextLength == 0)
            {

            }
            else
            {
                this.listBoxInterimFirstname.Items.Clear();
                this.listBoxInterimLastname.Items.Clear();
                this.listBoxInterimMissions.Items.Clear();
                this.listBoxInterimPhone.Items.Clear();
                this.listBoxInterimSkills.Items.Clear();
                this.listBoxInterimSalary.Items.Clear();

                if (this.comboBoxSearchInterim.Text == "Firstname")
                {
                    foreach (EmployeInterim item in ListInterimsTemp)
                    {
                        if (item.firstname.ToUpper() == this.textBoxSearchInterim.Text.ToUpper())
                        {
                            test = true;
                            this.listBoxInterimFirstname.Items.Add(item.firstname);
                            this.listBoxInterimLastname.Items.Add(item.lastname);
                            this.listBoxInterimPhone.Items.Add(item.telephoneNumber);
                            this.listBoxInterimSkills.Items.Add(item.GetSkills());
                            this.listBoxInterimMissions.Items.Add(item.GetMissions());
                            this.listBoxInterimSalary.Items.Add(item.GetSalary());
                        }
                    }

                    if (!test)
                        this.listBoxInterimFirstname.Items.Add("No match");
                }

                if (this.comboBoxSearchInterim.Text == "Lastname")
                {
                    foreach (EmployeInterim item in ListInterimsTemp)
                    {
                        if (item.lastname.ToUpper() == this.textBoxSearchInterim.Text.ToUpper())
                        {
                            test = true;
                            this.listBoxInterimFirstname.Items.Add(item.firstname);
                            this.listBoxInterimLastname.Items.Add(item.lastname);
                            this.listBoxInterimPhone.Items.Add(item.telephoneNumber);
                            this.listBoxInterimSkills.Items.Add(item.GetSkills());
                            this.listBoxInterimMissions.Items.Add(item.GetMissions());
                        }
                    }

                    if (!test)
                        this.listBoxInterimFirstname.Items.Add("No match");
                }

            }
        }

        private void textBoxSearchInterim_KeyUp(object sender, KeyEventArgs e)
        {

                bool test = false;
                int nb = this.textBoxSearchInterim.TextLength;
                if (this.textBoxSearchInterim.TextLength == 0)
                {
                    DisplayInterim();
                }
                else
                {
                    this.listBoxInterimFirstname.Items.Clear();
                    this.listBoxInterimLastname.Items.Clear();
                    this.listBoxInterimMissions.Items.Clear();
                    this.listBoxInterimPhone.Items.Clear();
                    this.listBoxInterimSkills.Items.Clear();
                    this.listBoxInterimSalary.Items.Clear();

                    if (this.comboBoxSearchInterim.Text == "Firstname")
                    {
                        foreach (EmployeInterim item in ListInterimsTemp)
                        {
                            try
                            {
                                if (item.firstname.ToUpper().Substring(0, nb) == this.textBoxSearchInterim.Text.ToUpper().Substring(0, nb))
                                {
                                    test = true;
                                    this.listBoxInterimFirstname.Items.Add(item.firstname);
                                    this.listBoxInterimLastname.Items.Add(item.lastname);
                                    this.listBoxInterimPhone.Items.Add(item.telephoneNumber);
                                    this.listBoxInterimSkills.Items.Add(item.GetSkills());
                                    this.listBoxInterimMissions.Items.Add(item.GetMissions());
                                    this.listBoxInterimSalary.Items.Add(item.GetSalary());
                                }
                            }
                            catch (ArgumentOutOfRangeException exception)
                            {

                            }
                        }

                        if (!test)
                            this.listBoxInterimFirstname.Items.Add("No match");
                    }

                    if (this.comboBoxSearchInterim.Text == "Lastname")
                    {
                        foreach (EmployeInterim item in ListInterimsTemp)
                        {
                            try
                            {
                                if (item.lastname.ToUpper().Substring(0, nb) == this.textBoxSearchInterim.Text.ToUpper().Substring(0, nb))
                                {
                                    test = true;
                                    this.listBoxInterimFirstname.Items.Add(item.firstname);
                                    this.listBoxInterimLastname.Items.Add(item.lastname);
                                    this.listBoxInterimPhone.Items.Add(item.telephoneNumber);
                                    this.listBoxInterimSkills.Items.Add(item.GetSkills());
                                    this.listBoxInterimMissions.Items.Add(item.GetMissions());
                                    this.listBoxInterimSalary.Items.Add(item.GetSalary());
                                }
                            }
                            catch (ArgumentOutOfRangeException exception)
                            {

                            }
                        }

                        if (!test)
                            this.listBoxInterimFirstname.Items.Add("No match");
                    }

                }
        }

        private void textBoxSearchEnterprise_KeyUp(object sender, KeyEventArgs e)
        {
            bool test = false;
            int nb = this.textBoxSearchEnterprise.TextLength;
            if (this.textBoxSearchEnterprise.TextLength == 0)
            {
                DisplayEnterprise();
            }
            else
            {
                this.listBoxEnterpriseName.Items.Clear();
                this.listBoxEnterpriseAdress.Items.Clear();
                this.listBoxEnterpriseSiret.Items.Clear();
                this.listBoxContactFirstname.Items.Clear();
                this.listBoxContactLastname.Items.Clear();
                this.listBoxContactPhone.Items.Clear();

                if (this.comboBoxSearchEnterprise.Text == "Name")
                {
                    foreach (Enterprise item in ListEnterprisesTemp)
                    {
                        try
                        {
                            if (item.name.ToUpper().Substring(0, nb) == this.textBoxSearchEnterprise.Text.ToUpper().Substring(0, nb))
                            {
                                test = true;
                                this.listBoxEnterpriseName.Items.Add(item.name);
                                this.listBoxEnterpriseAdress.Items.Add(item.adress);
                                this.listBoxEnterpriseSiret.Items.Add(item.siret);
                                this.listBoxContactFirstname.Items.Add(item.innermate.firstname);
                                this.listBoxContactLastname.Items.Add(item.innermate.lastname);
                                this.listBoxContactPhone.Items.Add(item.innermate.telephoneNumber);
                            }
                        }
                        catch (ArgumentOutOfRangeException exception)
                        {

                        }
                    }

                    if (!test)
                        this.listBoxEnterpriseName.Items.Add("No match");
                }

                if (this.comboBoxSearchEnterprise.Text == "Siret")
                {
                    foreach (Enterprise item in ListEnterprisesTemp)
                    {
                        try
                        {
                            if (item.siret.ToString().ToUpper().Substring(0, nb) == this.textBoxSearchEnterprise.Text.ToUpper().Substring(0, nb))
                            {
                                test = true;
                                this.listBoxEnterpriseName.Items.Add(item.name);
                                this.listBoxEnterpriseAdress.Items.Add(item.adress);
                                this.listBoxEnterpriseSiret.Items.Add(item.siret);
                                this.listBoxContactFirstname.Items.Add(item.innermate.firstname);
                                this.listBoxContactLastname.Items.Add(item.innermate.lastname);
                                this.listBoxContactPhone.Items.Add(item.innermate.telephoneNumber);
                            }
                        }
                        catch (ArgumentOutOfRangeException exception)
                        {

                        }
                    }

                    if (!test)
                        this.listBoxEnterpriseName.Items.Add("No match");
                }

                if (this.comboBoxSearchEnterprise.Text == "Contact's Lastname")
                {
                    foreach (Enterprise item in ListEnterprisesTemp)
                    {
                        try
                        {
                            if (item.innermate.lastname.ToUpper().Substring(0, nb) == this.textBoxSearchEnterprise.Text.ToUpper().Substring(0, nb))
                            {
                                test = true;
                                this.listBoxEnterpriseName.Items.Add(item.name);
                                this.listBoxEnterpriseAdress.Items.Add(item.adress);
                                this.listBoxEnterpriseSiret.Items.Add(item.siret);
                                this.listBoxContactFirstname.Items.Add(item.innermate.firstname);
                                this.listBoxContactLastname.Items.Add(item.innermate.lastname);
                                this.listBoxContactPhone.Items.Add(item.innermate.telephoneNumber);
                            }
                        }
                        catch (ArgumentOutOfRangeException exception)
                        {

                        }
                    }

                    if (!test)
                        this.listBoxEnterpriseName.Items.Add("No match");
                }

            }
        }

        private void buttonClearEnterprise_Click(object sender, EventArgs e)
        {
            DisplayEnterprise();
        }

        private void buttonSearchEnterprise_Click(object sender, EventArgs e)
        {
            bool test = false;
            int nb = this.textBoxSearchEnterprise.TextLength;
            if (this.textBoxSearchEnterprise.TextLength == 0)
            {
                DisplayEnterprise();
            }
            else
            {
                this.listBoxEnterpriseName.Items.Clear();
                this.listBoxEnterpriseAdress.Items.Clear();
                this.listBoxEnterpriseSiret.Items.Clear();
                this.listBoxContactFirstname.Items.Clear();
                this.listBoxContactLastname.Items.Clear();
                this.listBoxContactPhone.Items.Clear();

                if (this.comboBoxSearchEnterprise.Text == "Name")
                {
                    foreach (Enterprise item in ListEnterprisesTemp)
                    {
                        try
                        {
                            if (item.name.ToUpper().Substring(0, nb) == this.textBoxSearchEnterprise.Text.ToUpper().Substring(0, nb))
                            {
                                test = true;
                                this.listBoxEnterpriseName.Items.Add(item.name);
                                this.listBoxEnterpriseAdress.Items.Add(item.adress);
                                this.listBoxEnterpriseSiret.Items.Add(item.siret);
                                this.listBoxContactFirstname.Items.Add(item.innermate.firstname);
                                this.listBoxContactLastname.Items.Add(item.innermate.lastname);
                                this.listBoxContactPhone.Items.Add(item.innermate.telephoneNumber);
                            }
                        }
                        catch (ArgumentOutOfRangeException exception)
                        {

                        }
                    }

                    if (!test)
                        this.listBoxEnterpriseName.Items.Add("No match");
                }

                if (this.comboBoxSearchEnterprise.Text == "Siret")
                {
                    foreach (Enterprise item in ListEnterprisesTemp)
                    {
                        try
                        {
                            if (item.siret.ToString().ToUpper().Substring(0, nb) == this.textBoxSearchEnterprise.Text.ToUpper().Substring(0, nb))
                            {
                                test = true;
                                this.listBoxEnterpriseName.Items.Add(item.name);
                                this.listBoxEnterpriseAdress.Items.Add(item.adress);
                                this.listBoxEnterpriseSiret.Items.Add(item.siret);
                                this.listBoxContactFirstname.Items.Add(item.innermate.firstname);
                                this.listBoxContactLastname.Items.Add(item.innermate.lastname);
                                this.listBoxContactPhone.Items.Add(item.innermate.telephoneNumber);
                            }
                        }
                        catch (ArgumentOutOfRangeException exception)
                        {

                        }
                    }

                    if (!test)
                        this.listBoxEnterpriseName.Items.Add("No match");
                }

                if (this.comboBoxSearchEnterprise.Text == "Contact's Lastname")
                {
                    foreach (Enterprise item in ListEnterprisesTemp)
                    {
                        try
                        {
                            if (item.innermate.lastname.ToUpper().Substring(0, nb) == this.textBoxSearchEnterprise.Text.ToUpper().Substring(0, nb))
                            {
                                test = true;
                                this.listBoxEnterpriseName.Items.Add(item.name);
                                this.listBoxEnterpriseAdress.Items.Add(item.adress);
                                this.listBoxEnterpriseSiret.Items.Add(item.siret);
                                this.listBoxContactFirstname.Items.Add(item.innermate.firstname);
                                this.listBoxContactLastname.Items.Add(item.innermate.lastname);
                                this.listBoxContactPhone.Items.Add(item.innermate.telephoneNumber);
                            }
                        }
                        catch (ArgumentOutOfRangeException exception)
                        {

                        }
                    }

                    if (!test)
                        this.listBoxEnterpriseName.Items.Add("No match");
                }

            }
        }

        private void textBoxSearchMission_KeyUp(object sender, KeyEventArgs e)
        {
            bool test = false;
            int nb = this.textBoxSearchMission.TextLength;
            if (this.textBoxSearchMission.TextLength == 0)
            {
                DisplayMission();
            }
            else
            {
                this.listBoxMissionEnterprise.Items.Clear();
                this.listBoxMissionId.Items.Clear();
                this.listBoxMissionInterims.Items.Clear();
                this.listBoxMissionOutset.Items.Clear();
                this.listBoxMissionStatus.Items.Clear();
                this.listBoxMissionTermination.Items.Clear();

                if (this.comboBoxSearchMission.Text == "Client")
                {
                    foreach (Mission item in ListMissionsTemp)
                    {
                        try
                        {
                            if (item.enterpriseName.ToUpper().Substring(0, nb) == this.textBoxSearchMission.Text.ToUpper().Substring(0, nb))
                            {
                                test = true;
                                this.listBoxMissionEnterprise.Items.Add(item.enterpriseName);
                                this.listBoxMissionId.Items.Add(item.id);
                                this.listBoxMissionInterims.Items.Add(item.GetInterims());
                                this.listBoxMissionOutset.Items.Add(item.outset.ToString());
                                this.listBoxMissionStatus.Items.Add(item.GetStatus());
                                this.listBoxMissionTermination.Items.Add(item.termination.ToString());
                            }
                        }
                        catch (ArgumentOutOfRangeException exception)
                        {

                        }
                    }

                    if (!test)
                        this.listBoxMissionEnterprise.Items.Add("No match");
                }

                //if (this.comboBoxSearchMission.Text == "Interims")
                //{
                //    foreach (Mission item in ListMissionsTemp)
                //    {
                //        try
                //        {
                //            foreach (EmployeInterim interim in item.interims)
                //            {
                //                try
                //                {

                //                }
                //                catch (ArgumentOutOfRangeException exception)
                //                {
                                    
                                    
                //                }
                //            }
                //        }
                //        catch (ArgumentOutOfRangeException exception)
                //        {

                //        }
                //    }

                //    if (!test)
                //        this.listBoxMissionEnterprise.Items.Add("No match");
                //}

            }
        }

        private void buttonAddInterim_Click(object sender, EventArgs e)
        {
            if (this.textBoxAddInterimFirstname.TextLength > 0 && this.textBoxAddInterimLastname.TextLength > 0 && this.textBoxAddInterimPhone.TextLength > 0 && this.textBoxAddInterimSkills.TextLength > 0)
            {
                string[] skills = this.textBoxAddInterimSkills.Text.Split(';');
                this.ListInterimsTemp.Add(new EmployeInterim(this.textBoxAddInterimFirstname.Text, this.textBoxAddInterimLastname.Text, this.textBoxAddInterimPhone.Text, skills));
                this.textBoxAddInterimFirstname.Clear();
                this.textBoxAddInterimLastname.Clear();
                this.textBoxAddInterimPhone.Clear();
                this.textBoxAddInterimSkills.Clear();
                DisplayInterim();
            }
        }

        private void toolStripMenuItemSave_Click(object sender, EventArgs e)
        {
            //Serializer.Serialize("interims.o", this.ListInterimsTemp);
            //Serializer.Serialize("enterprises.o", this.ListEnterprisesTemp);
            //Serializer.Serialize("missions.o", this.ListMissionsTemp);
        }

    }
}
