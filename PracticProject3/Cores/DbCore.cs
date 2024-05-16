using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using PracticProject3.DownloadData;
using PracticProject3.Dates;
using PracticProject3.Cores;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Data;
using DocumentFormat.OpenXml.Spreadsheet;

namespace PracticProject3.Cores
{
    public enum AuthResponse {HasProfile,NotFound,StartUse}
    static public class DbCore
    {
        static string ConnectionString = "";
        static string ServerName = "";
        static string DbName = "";
        static string UserId = "";
        static string Password = "";
        static string OtherParams = "";
        public static SqlConnection ConnectLine;
        static public void ConnectionSettings(string _server,string _db,string _user,string _password,string _other) 
        {
            ServerName = _server;
            DbName = _db;
            UserId = _user;
            Password = _password;
            OtherParams = _other;
            ConnectionString = $"Server={_server};Database={_db};User Id={_user};Password={_password};{_other}";
        }
        static public void CreateConnection()
        {
            if (ConnectLine != null) 
            {
                ConnectLine.Close();
            }
            ConnectLine = new SqlConnection(ConnectionString);
        }
        static public void CreateConnection(string ConnectString)
        {
            if (ConnectLine != null)
            {
                ConnectLine.Close();
            }
            ConnectionString = ConnectString;
            ConnectLine = new SqlConnection(ConnectionString);
        }


        // Далее команды для отправки и получения данных

        static async public Task AddCompany(string NameNum,string Name) 
        {
            //await ConnectLine.OpenAsync();
            string AddCommand = @"
            INSERT dbo.Companies(TypeNum,Name,Status) 
            VALUES
            (@type,@name,'Work')";
            SqlCommand cWrite = new SqlCommand(AddCommand, ConnectLine);
            cWrite.Parameters.Add(new SqlParameter("@type", NameNum));
            cWrite.Parameters.Add(new SqlParameter("@name", Name));
            await cWrite.ExecuteNonQueryAsync();
            //ConnectLine.Close();
        }

        static async public Task RemoveCompany(int Id)
        {
            //await ConnectLine.OpenAsync();
            string RemoveCommand = @"
            UPDATE dbo.Companies
            SET dbo.Companies.Status = 'Deleted'
            WHERE dbo.Companies.Id = @id
            ";
            SqlCommand cWrite = new SqlCommand(RemoveCommand, ConnectLine);
            cWrite.Parameters.Add(new SqlParameter("@id", Id));
            await cWrite.ExecuteNonQueryAsync();
            //ConnectLine.Close();
        }

        static async public Task AddCorpus(string NameNum, string Name)
        {
            //await ConnectLine.OpenAsync();
            string AddCommand = @"
            INSERT dbo.Corpuses(Name,NameNum,Status) 
            VALUES
            (@name,@num_name,'Work')";
            SqlCommand cWrite = new SqlCommand(AddCommand, ConnectLine);
            cWrite.Parameters.Add(new SqlParameter("@name", Name));
            cWrite.Parameters.Add(new SqlParameter("@num_name", NameNum));
            await cWrite.ExecuteNonQueryAsync();
            //ConnectLine.Close();
        }

        static async public Task RemoveCorpus(int Id)
        {
            //await ConnectLine.OpenAsync();
            string RemoveCommand = @"
            UPDATE dbo.Corpuses
            SET dbo.Corpuses.Status = 'Deleted'
            WHERE dbo.Corpuses.Id = @id
            ";
            SqlCommand cWrite = new SqlCommand(RemoveCommand, ConnectLine);
            cWrite.Parameters.Add(new SqlParameter("@id", Id));
            await cWrite.ExecuteNonQueryAsync();
            //ConnectLine.Close();
        }

        static async public Task AddDocType(string FullName, string Name)
        {
            //await ConnectLine.OpenAsync();
            string AddCommand = @"
            INSERT dbo.DocTypes(Name,FullName,Status) 
            VALUES
            (@name,@full_name,'Work')";
            SqlCommand cWrite = new SqlCommand(AddCommand, ConnectLine);
            cWrite.Parameters.Add(new SqlParameter("@name", Name));
            cWrite.Parameters.Add(new SqlParameter("@full_name", FullName));
            await cWrite.ExecuteNonQueryAsync();
            //ConnectLine.Close();
        }

        static async public Task RemoveDocType(int Id)
        {
            //await ConnectLine.OpenAsync();
            string RemoveCommand = @"
            UPDATE dbo.DocTypes
            SET dbo.DocTypes.Status = 'Deleted'
            WHERE dbo.DocTypes.Id = @id
            ";
            SqlCommand cWrite = new SqlCommand(RemoveCommand, ConnectLine);
            cWrite.Parameters.Add(new SqlParameter("@id", Id));
            await cWrite.ExecuteNonQueryAsync();
            //ConnectLine.Close();
        }

        static async public Task DownloadDocumentsList()
        {
            List<Record> doc_list = new List<Record>();
            //await ConnectLine.OpenAsync();
            string DownloadCommand = @"
            SELECT dbo.Documents.DocNum,dbo.DocTypes.[Name],dbo.DocTypes.[FullName],dbo.Companies.[Name],dbo.Companies.TypeNum,dbo.Corpuses.[Name],dbo.Corpuses.[NameNum],dbo.Documents.RegTime,dbo.Documents.Tags 
            FROM dbo.Documents
            JOIN dbo.Companies
            ON dbo.Documents.CompanyId = dbo.Companies.Id and dbo.Companies.Status = 'Work'
            JOIN dbo.DocTypes
            ON dbo.Documents.DocTypeId = dbo.DocTypes.Id and dbo.DocTypes.Status = 'Work'
            JOIN dbo.Corpuses
            ON dbo.Documents.CorpusId = dbo.Corpuses.Id and dbo.Corpuses.Status = 'Work'
            JOIN dbo.Users
            ON dbo.Documents.UserId = dbo.Users.Id and dbo.Users.Status = 'Work'
            WHERE dbo.Documents.Status = 'Work'
            ";
            SqlCommand cReadDocuments = new SqlCommand(DownloadCommand, ConnectLine);
            SqlDataReader reader = await cReadDocuments.ExecuteReaderAsync();
            Record obj;
            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    obj = new Record(
                        reader.GetString(0),
                        reader.GetString(1),
                        reader.GetString(2),
                        reader.GetString(3),
                        reader.GetString(4),
                        reader.GetString(5),
                        reader.GetString(6),
                        reader.GetDateTime(7).ToString(),
                        reader.GetString(8)
                        );
                    doc_list.Add(obj);
                }
            }
            reader.Close();
            //System.Windows.Forms.MessageBox.Show("!!");
            //ConnectLine.Close();
            DecodeCore.Records = doc_list;
        }

        // Обновление Decode данных (скачивание актуальных компаний, корпусов, типов документации)
        static async public Task DownloadData() 
        {
            DecodeCore.ClearData();
            //await ConnectLine.OpenAsync();
            string DownloadCommand = @"
            SELECT * 
            FROM dbo.Corpuses
            WHERE dbo.Corpuses.Status = 'Work'
            ";
            SqlCommand cReadCorpus = new SqlCommand(DownloadCommand, ConnectLine);
            SqlDataReader reader = await cReadCorpus.ExecuteReaderAsync();
            if (reader.HasRows) 
            {
                while (await reader.ReadAsync()) 
                {
                    DecodeCore.Corpuses.Add(new Corpus(reader.GetInt32(0), reader.GetString(2), reader.GetString(1)));
                }
            }
            reader.Close();
            DownloadCommand = @"
            SELECT * 
            FROM dbo.DocTypes
            WHERE dbo.DocTypes.Status = 'Work'
            ";
            SqlCommand cReadDocType = new SqlCommand(DownloadCommand, ConnectLine);
            reader = await cReadDocType.ExecuteReaderAsync();
            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    DecodeCore.DocTypes.Add(new DocType(reader.GetInt32(0), reader.GetString(2), reader.GetString(1)));
                }
            }
            reader.Close();
            DownloadCommand = @"
            SELECT * 
            FROM dbo.Companies
            WHERE dbo.Companies.Status = 'Work'
            ";
            SqlCommand cReadCompany = new SqlCommand(DownloadCommand, ConnectLine);
            reader = await cReadCompany.ExecuteReaderAsync();
            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    DecodeCore.Companies.Add(new Company(reader.GetInt32(0), reader.GetString(1), reader.GetString(2)));
                }
            }
            reader.Close();
            //ConnectLine.Close();
        }

        // Отправление пакетов в БД (загрузка новых записей)
        static async public Task SendData(int UserId, List<InfoData> ID) 
        {
            //await ConnectLine.OpenAsync();
            string AddCommand = @"
            INSERT dbo.Documents(SysPath,SysType,SysHash,DocNum,CompanyId,DocTypeId,CorpusId,UserId,Tags,RegTime,Status) 
            VALUES
            ";
            for (int i = 0; i < ID.Count; i++)
            {
                AddCommand += $"(@sysPath{i},@sysType{i},@sysHash{i},@docNum{i},@companyId{i}," +
                    $"@docTypeId{i},@corpusId{i},@userId{i},@tags{i},@regTime{i},'Work'),";
            }
            AddCommand = AddCommand.Substring(0, AddCommand.Length - 1) + ';';
            SqlCommand cWrite = new SqlCommand(AddCommand, ConnectLine);
            for (int i = 0; i < ID.Count; i++)
            {
                cWrite.Parameters.Add(new SqlParameter($"@sysPath{i}", ID[i].SysPath));
                cWrite.Parameters.Add(new SqlParameter($"@sysType{i}",ID[i].SysType));
                cWrite.Parameters.Add(new SqlParameter($"@sysHash{i}",ID[i].SysHash));
                cWrite.Parameters.Add(new SqlParameter($"@docNum{i}",ID[i].DocNum));
                cWrite.Parameters.Add(new SqlParameter($"@companyId{i}", ID[i].Company.Id));
                cWrite.Parameters.Add(new SqlParameter($"@docTypeId{i}", ID[i].Type.Id));
                cWrite.Parameters.Add(new SqlParameter($"@corpusId{i}", ID[i].Corpus.Id));
                cWrite.Parameters.Add(new SqlParameter($"@userId{i}", UserId));
                cWrite.Parameters.Add(new SqlParameter($"@tags{i}", ID[i].Tags));
                cWrite.Parameters.Add(new SqlParameter($"@regTime{i}", DateTime.Parse(ID[i].RegTime)));
            }
            //System.Windows.Forms.MessageBox.Show(AddCommand);
            await cWrite.ExecuteNonQueryAsync();
            //ConnectLine.Close();
        }

        // Создание структуры БД (первоначальный запуск; в иных случаях не создавать)
        static async public Task BuildDbStruct(string DBName) 
        {
            // DecryptosBase
            //await ConnectLine.OpenAsync();
            string AddCommand1 = $@"
            IF DB_ID('{DBName}') IS NULL   
            BEGIN
	            CREATE DATABASE {DBName}
            END
            ;";
            string AddCommand2 = $@"
            USE {DBName}
            IF OBJECT_ID('{DBName}.dbo.Documents') IS NULL
            BEGIN
	            CREATE TABLE dbo.Documents
	            (
	            Id INT PRIMARY KEY IDENTITY(1,1),
	            SysPath NVARCHAR(256) NOT NULL,
                SysType NVARCHAR(5) NOT NULL,
                SysHash NVARCHAR(35) NOT NULL UNIQUE,
                DocNum NVARCHAR(50) NOT NULL,
                CompanyId INT NOT NULL,
                DocTypeId INT NOT NULL,
                CorpusId INT NOT NULL,
                UserId INT NOT NULL,
                Tags NVARCHAR(100) NOT NULL,
                RegTime DATETIME NOT NULL,
                [Status] NVARCHAR(15) NOT NULL
	            )
            END
            IF OBJECT_ID('{DBName}.dbo.Companies') IS NULL 
            BEGIN
	            CREATE TABLE dbo.Companies
	            (
	            Id INT PRIMARY KEY IDENTITY(1,1),
	            TypeNum NVARCHAR(10) NOT NULL,
                [Name] NVARCHAR(50) NOT NULL,
                [Status] NVARCHAR(15) NOT NULL
	            )
            END
            IF OBJECT_ID('{DBName}.dbo.Corpuses') IS NULL 
            BEGIN
	            CREATE TABLE dbo.Corpuses
	            (
	            Id INT PRIMARY KEY IDENTITY(1,1),
                [Name] NVARCHAR(10) NOT NULL,
                [NameNum] NVARCHAR(10) NOT NULL,
                [Status] NVARCHAR(15) NOT NULL
	            )
            END
            IF OBJECT_ID('{DBName}.dbo.DocTypes') IS NULL 
            BEGIN
	            CREATE TABLE dbo.DocTypes
	            (
	            Id INT PRIMARY KEY IDENTITY(1,1),
                [Name] NVARCHAR(50) NOT NULL,
                [FullName] NVARCHAR(60) NOT NULL,
                [Status] NVARCHAR(15) NOT NULL
	            )
            END
            IF OBJECT_ID('{DBName}.dbo.Permissions') IS NULL 
            BEGIN
	            CREATE TABLE dbo.[Permissions]
	            (
	            Id INT PRIMARY KEY IDENTITY(1,1),
                [Name] NVARCHAR(30) NOT NULL,
                [Status] NVARCHAR(15) NOT NULL
	            )
                INSERT INTO dbo.[Permissions]([Name],[Status])
                VALUES
                ('Owner','Work'),('User','Work'),('Admin','Work')
            END
            IF OBJECT_ID('{DBName}.dbo.Users') IS NULL 
            BEGIN
	            CREATE TABLE dbo.Users
	            (
	            Id INT PRIMARY KEY IDENTITY(1,1),
                [FirstName] NVARCHAR(30) NOT NULL,
	            [SecondName] NVARCHAR(30) NOT NULL,
	            [ThirdName] NVARCHAR(30) NOT NULL,
	            [Login] NVARCHAR(50) NOT NULL UNIQUE,
                [Password] NVARCHAR(120) NOT NULL,
	            [PermissionId] INT NOT NULL,
                [Status] NVARCHAR(15) NOT NULL
	            )
            END
            IF OBJECT_ID('dbo.Logs') IS NULL 
            BEGIN
	            CREATE TABLE dbo.Logs
	            (
	            Id INT PRIMARY KEY IDENTITY(1,1),
                [UserId] INT NOT NULL,
	            [LogType] NVARCHAR(20) NOT NULL,
	            [Description] NVARCHAR(200) NOT NULL,
                [ActionTime] DATETIME NOT NULL,
                [Status] NVARCHAR(15) NOT NULL
	            )
            END;
            "; // добавить параметр пароля
            SqlCommand cWrite1 = new SqlCommand(AddCommand1, ConnectLine);
            SqlCommand cWrite2 = new SqlCommand(AddCommand2, ConnectLine);
            await cWrite1.ExecuteNonQueryAsync();
            await cWrite2.ExecuteNonQueryAsync();
            //ConnectLine.Close();
        }
        // Переключение в профиль БД в соответствии с правами

        static async public Task<AuthResponse> Auth(string Login,string Password) 
        {
            string CheckCommand = "SELECT dbo.Users.Id FROM dbo.Users;";
            string AuthCommand = @"
            SELECT dbo.Users.Id,dbo.Users.FirstName,dbo.Users.SecondName,dbo.Users.ThirdName, dbo.[Permissions].[Name]
            FROM dbo.Users
            JOIN dbo.[Permissions]
            ON dbo.Users.PermissionId = dbo.[Permissions].Id AND dbo.[Permissions].Status = 'Work'
            WHERE dbo.Users.Status = 'Work' AND dbo.Users.[Login] = @login AND dbo.Users.[Password] = @password; 
            "; // добавить пароль
            SqlCommand cReadAuth = new SqlCommand(AuthCommand, ConnectLine);
            SqlCommand cReadCheck = new SqlCommand(CheckCommand, ConnectLine);
            cReadAuth.Parameters.Add(new SqlParameter("@login", Login));
            cReadAuth.Parameters.Add(new SqlParameter("@password", Settings.ToHashSHA256(Password)));
            SqlDataReader reader = await cReadCheck.ExecuteReaderAsync();
            if (reader.HasRows == false) 
            {
                reader.Close();
                return AuthResponse.StartUse; 
            }
            reader.Close();
            reader = await cReadAuth.ExecuteReaderAsync();
            if (reader.HasRows)
            {
                await reader.ReadAsync();
                Settings.FirstName = reader.GetString(1);
                Settings.SecondName = reader.GetString(2);
                Settings.ThirdName = reader.GetString(3);
                Settings.UserId = reader.GetInt32(0);
                Settings.Permission = reader.GetString(4);
                reader.Close();
                return AuthResponse.HasProfile;
            }
            else 
            {
                reader.Close();
                return AuthResponse.NotFound;
            }
        }

        static async public Task Log(string Type,string Description)
        {
            //await ConnectLine.OpenAsync();
            string LogCommand = @"
            INSERT dbo.Logs(UserId,LogType,Description,ActionTime,Status) 
            VALUES
            (@id,@type,@desc,@date,'Work')";
            SqlCommand cWrite = new SqlCommand(LogCommand, ConnectLine);
            cWrite.Parameters.Add(new SqlParameter("@id", Settings.UserId));
            cWrite.Parameters.Add(new SqlParameter("@type", Type));
            cWrite.Parameters.Add(new SqlParameter("@desc", Description));
            cWrite.Parameters.Add(new SqlParameter("@date", DateTime.Now));
            await cWrite.ExecuteNonQueryAsync();
            //ConnectLine.Close();
        }

        static async public Task GetUsers()
        {
            Settings.Users.Clear();
            string DownloadCommand = @"
            SELECT [Id],[FirstName],[SecondName],[ThirdName]
            FROM dbo.Users
            WHERE dbo.Users.Status = 'Work'
            ";
            SqlCommand cRead = new SqlCommand(DownloadCommand, ConnectLine);
            SqlDataReader reader = await cRead.ExecuteReaderAsync();
            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    Settings.Users.Add(new User(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3)));
                }
            }
            reader.Close();
        }

        static async public Task GetPermissions()
        {
            Settings.Permissions.Clear();
            string DownloadCommand = @"
            SELECT [Id],[Name]
            FROM dbo.Permissions
            WHERE dbo.Permissions.Status = 'Work'
            ";
            SqlCommand cRead = new SqlCommand(DownloadCommand, ConnectLine);
            SqlDataReader reader = await cRead.ExecuteReaderAsync();
            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    Settings.Permissions.Add(new Permission(reader.GetInt32(0), reader.GetString(1)));
                }
            }
            reader.Close();
        }

        static async public Task ChangePermissions(int UserId,int PermId)
        {
            //await ConnectLine.OpenAsync();
            string ChangeCommand = @"
            UPDATE dbo.Users
            SET dbo.Users.[PermissionId] = @permId
            WHERE dbo.Users.[Id] = @id";
            SqlCommand cWrite = new SqlCommand(ChangeCommand, ConnectLine);
            cWrite.Parameters.Add(new SqlParameter("@id", UserId));
            cWrite.Parameters.Add(new SqlParameter("@permId", PermId));
            await cWrite.ExecuteNonQueryAsync();
            //ConnectLine.Close();
        }

        static async public Task DeleteUser(int UserId)
        {
            //await ConnectLine.OpenAsync();
            string DeleteCommand = @"
            UPDATE dbo.Users
            SET dbo.Users.[Status] = 'Deleted'
            WHERE dbo.Users.[Id] = @id";
            SqlCommand cWrite = new SqlCommand(DeleteCommand, ConnectLine);
            cWrite.Parameters.Add(new SqlParameter("@id", UserId));
            await cWrite.ExecuteNonQueryAsync();
            //ConnectLine.Close();
        }

        static async public Task AddUser(string FirstName, string SecondName, string ThirdName, string Login, string Password, int PermId)
        {
            //await ConnectLine.OpenAsync();
            string AddCommand = @"
            INSERT dbo.Users(FirstName,SecondName,ThirdName,Login,Password,PermissionId,Status) 
            VALUES
            (@firstName,@secondName,@thirdName,@login,@password,@permId,'Work')";
            SqlCommand cWrite = new SqlCommand(AddCommand, ConnectLine);
            cWrite.Parameters.Add(new SqlParameter("@firstName", FirstName));
            cWrite.Parameters.Add(new SqlParameter("@secondName", SecondName));
            cWrite.Parameters.Add(new SqlParameter("@thirdName", ThirdName));
            cWrite.Parameters.Add(new SqlParameter("@login", Login));
            cWrite.Parameters.Add(new SqlParameter("@password", Settings.ToHashSHA256(Password)));
            cWrite.Parameters.Add(new SqlParameter("@permId", PermId));
            await cWrite.ExecuteNonQueryAsync();
            //ConnectLine.Close();
        }

        static public async void CreateDbDataTable(List<DataPack> data_obj)
        {
            int StrCount = data_obj.Sum(x=>x.Data.Count);
            List<string> TableNames = new List<string>();
            List<string> LineIds = new List<string>();
            List<string> LineErrors = new List<string>();
            ProgressList pro_menu = new ProgressList(StrCount - data_obj.Count);
            pro_menu.Show();
            await ConnectLine.OpenAsync(); // USE DecryptosBase
            for (int i = 0; i < data_obj.Count; i++)
            {
                //data_obj[i].DataPackName
                string str1 = $" CREATE TABLE dbo.[{data_obj[i].DataPackName}](";// + "Id INT PRIMARY KEY IDENTITY(1,1),";
                string str1_add = $"SET IDENTITY_INSERT dbo.[{data_obj[i].DataPackName}] ON;";
                for (int h1 = 0; h1 < data_obj[i].Data[0].line.Count; h1++)
                {
                    if (h1 == 0)
                    {
                        str1 += $"[{data_obj[i].Data[0].line[h1]}] INT PRIMARY KEY ";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(data_obj[i].Data[0].line[h1].ToString()))
                        {
                            str1 += $",[{data_obj[i].Data[0].line[h1]}] NVARCHAR(MAX) NULL";
                        }
                        else 
                        {
                            str1 += $",[Столбик{h1}] NVARCHAR(MAX) NULL";
                        }
                    }
                }
                str1 += ");";
                SqlCommand command1 = new SqlCommand(str1, ConnectLine);
                try
                {
                    await command1.ExecuteNonQueryAsync();
                    command1.CommandText = str1_add;
                    await command1.ExecuteNonQueryAsync();
                }
                catch (SqlException ex)
                {

                }
                string str2 = $"INSERT dbo.[{data_obj[i].DataPackName}]" +
                "VALUES";
                for (int j = 1; j < data_obj[i].Data.Count; j++)
                {
                    SqlCommand command2 = new SqlCommand();
                    command2.Connection = ConnectLine;
                    string str3 = "";
                    for (int k = 0; k < data_obj[i].Data[j].line.Count; k++)
                    {
                        try
                        {
                            str3 += $",@arg{k}";
                            var param = new SqlParameter($"@arg{k}", SqlDbType.NVarChar);
                            if (!string.IsNullOrEmpty(data_obj[i].Data[j].line[k].ToString()))
                            {
                                param.Value = data_obj[i].Data[j].line[k];
                            }
                            else
                            {
                                param.Value = DBNull.Value;
                            }
                            command2.Parameters.Add(param);
                        }
                        catch
                        {
                            MessageBox.Show("Ошибка с ячейкой");
                        }
                    }
                    command2.CommandText = str2 + "(" + str3.Substring(1) + ");";
                    try
                    {
                        await command2.ExecuteNonQueryAsync();
                        pro_menu.AddProgress();
                    }
                    catch (SqlException ex)
                    {
                        TableNames.Add(data_obj[i].DataPackName);
                        LineIds.Add(data_obj[i].Data[j].line[0]);
                        LineErrors.Add(ex.Message);
                        //MessageBox.Show(ex.Message);
                    }
                }
            }
            //command1.Parameters.Add(new SqlParameter());
            ConnectLine.Close();
            pro_menu.AddList(TableNames, LineIds, LineErrors);
            MessageBox.Show("Готово!");
        }
        // додумать

        // Для проверок: System.Windows.Forms.MessageBox.Show("1");
    }
}


