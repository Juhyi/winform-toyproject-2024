using MetroFramework.Forms;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Cryptography;
using System.Text;



namespace schoolGradesManagementApp
{
    public partial class LoginForm : MetroForm
    {
        private bool isLogin = false;

        public MainForm curParent = null;
        
        public bool IsLogin
        {
            get { return isLogin; }
            set { isLogin = value; }
        }

        public LoginForm()
        {
            InitializeComponent();

            string empty = string.Empty;
            TxtUserId.Text = empty;
            TxtPassword.Text = string.Empty;
        }



        #region "�α���, ��ҹ�ư �̺�Ʈ�ڵ鷯"
        // ��ҹ�ư Ŭ�� �̺�Ʈ�ڵ鷯
        private void BtnCancle_Click(object sender, EventArgs e)
        {
            Environment.Exit(0); // ��� ������ ������ �����.
        }

        // �α��� Ŭ�� �̺�Ʈ�ڵ鷯

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            bool isFail = false;
            string errMsg = string.Empty;

            MainForm mainForm = new MainForm();

            if (string.IsNullOrEmpty(TxtUserId.Text))
            {
                isFail = true;
                errMsg += "�й��� �Է��ϼ���.\n";
            }

            if (string.IsNullOrEmpty(TxtPassword.Text))
            {
                isFail = true;
                errMsg += "Password�� �Է��ϼ���.\n";
            }

            if (isFail == true)
            {
                MessageBox.Show(errMsg, "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // DB����
            IsLogin = LoginProcess();   // �α����� �����ϸ� True, �����ϸ� False
            if (IsLogin)
            {
                if (Helper.Common.IsLogout == false) { // ���� �α��ν�
                    this.Close();
                } 
                else // �α׾ƿ� �� ��α���
                {
                    curParent.Show();
                    this.Close();
                }
            }
        }
        #endregion



        // �α��� DB���� ����
        private bool LoginProcess()
        {
            var md5Hash = MD5.Create();

            string userId = TxtUserId.Text;
            string password = TxtPassword.Text;
            string chkUserId = string.Empty;
            string chkPassword = string.Empty;



            /*
             * 1. Connection ����, ����
             * 2. ���� ���ڿ� �ۼ�
             * 3. SqlCommand ��� ��ü ����
             * 4. SqlParameter ��ü ����
             * 5. Select SqlDataReader �Ǵ� SqlDataSet ��ü ���
             * 6. CUD �۾� SqlCommand.ExecuteQuery()
             * 7. Connection �ݱ�
             */

            using (SqlConnection conn = new SqlConnection(Helper.Common.ConnString))
            {
                conn.Open();

                string query = @"SELECT userid
                                      ,[password]
                                   FROM usertbl
                                  WHERE userId = @userId
                                    AND [password] = @password";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlParameter prmUserId = new SqlParameter("@userId", userId);
                //SqlParameter prmPassword = new SqlParameter("@Password", Helper.Common.GetMD5Hash(md5Hash, password));
                SqlParameter prmPassword = new SqlParameter("@Password", password);

                cmd.Parameters.Add(prmUserId);
                cmd.Parameters.Add(prmPassword);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    chkUserId = reader["userId"] != null ? reader["userId"].ToString() : "-";   // �������̵� null�϶� = ����
                    chkPassword = reader["password"] != null ? reader["password"].ToString() : "-";
                    Helper.Common.LoginId = chkUserId;  // �α��ε� ���̵�.



                    return true;
                }
                else
                {
                    MessageBox.Show("�α��� ������ �����ϴ�.", "DB����", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return false;
                }


            }
        }


        private void TxtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                BtnLogin_Click(sender, e);
            }
        }

        private void TxtUserId_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                TxtPassword.Focus();  //�н������ ��Ŀ�� �̵�
            }
        }

        // ��й�ȣ ���� Ŭ��
        private void ChkShowPass_CheckedChanged(object sender, EventArgs e)
        {
            if (ChkShowPass.Checked == true)
            {
                TxtPassword.PasswordChar = default(char);
            }
            else
            {
                TxtPassword.PasswordChar = '��';
            }
        }

     
    }   
}
