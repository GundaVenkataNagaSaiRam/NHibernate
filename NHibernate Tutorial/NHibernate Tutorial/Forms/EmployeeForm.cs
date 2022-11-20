using NHibernate;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NHibernate_Tutorial.Forms
{
    public partial class EmployeeForm : Form
    {
        public EmployeeForm()
        {
            InitializeComponent();
        }

        
        private void EmployeeForm_Load(object sender, EventArgs e)
        {
            LoadEmployeeData();
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            LoadEmployeeData();
        }

        private void LoadEmployeeData()
        {
            ISession session = SessionFactory.OpenSession;

            using (session)
            {
                IQuery query = session.CreateQuery("FROM Employee");
                IList<Model.Employee> empInfo = query.List<Model.Employee>();
                dgViewEmployee.DataSource = empInfo;
            }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            Model.Employee empData = new Model.Employee();

            SetEmployeInfo(empData);

            ISession session = SessionFactory.OpenSession;

            using (session)
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        session.Save(empData);
                        transaction.Commit();
                        LoadEmployeeData();

                        tFirstName.Text = "";
                        tLastName.Text = "";
                        tEmail.Text = "";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        System.Windows.Forms.MessageBox.Show(ex.Message);
                        throw ex;
                    }
                }
            }
        }

        private void SetEmployeInfo(Model.Employee emp)
        {
            emp.FirstName = tFirstName.Text;
            emp.LastName = tLastName.Text;
            emp.Email = tEmail.Text;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            //to update data we will load current data to our textbox and then update
            ISession session = SessionFactory.OpenSession;

            using (session)
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        IQuery query = session.CreateQuery("FROM Employee WHERE Id = '" + IdTxtBx.Text + "'");
                        Model.Employee empData = query.List<Model.Employee>()[0];
                        SetEmployeInfo(empData);
                        session.Update(empData);
                        transaction.Commit();

                        LoadEmployeeData();

                        IdTxtBx.Text = "";
                        tFirstName.Text = "";
                        tLastName.Text = "";
                        tEmail.Text = "";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        private void dgViewEmployee_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgViewEmployee.RowCount <= 1 || e.RowIndex < 0)
                return;
            string id = dgViewEmployee[0, e.RowIndex].Value.ToString();

            if (id == "")
                return;

            IList<Model.Employee> empInfo = GetDataFromEmployee(id);

            IdTxtBx.Text = empInfo[0].Id.ToString();
            tFirstName.Text = empInfo[0].FirstName.ToString();
            tLastName.Text = empInfo[0].LastName.ToString();
            tEmail.Text = empInfo[0].Email.ToString();
        }

        private IList<Model.Employee> GetDataFromEmployee(string id)
        {
            ISession session = SessionFactory.OpenSession;

            using (session)
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        IQuery query = session.CreateQuery("FROM Employee WHERE Id = '" + id+ "'");
                        return query.List<Model.Employee>();
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show(ex.Message);
                        throw ex;
                    }
                }
            }
        }
    }
}
