class Program
{
    static List<User> users = new List<User>();
    static List<Doctor> doctors = new List<Doctor>();
    static List<Patient> patients = new List<Patient>();
    static User authenticatedUser;

    static ChiefDoctor chiefDoctor = new ChiefDoctor();

    static int lastDoctorId = 0;
    static int lastPatientId = 0;
    static int lastDiagnosisId = 0;
    static int lastMedicationId = 0;

    class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }

    class Person
    {
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string ContactInfo { get; set; }
    }

    class Patient : Person
    {
        public int Id { get; set; }
        public Doctor AttendingDoctor { get; set; }
        public List<Diagnosis> Diagnoses { get; set; } = new List<Diagnosis>();
        public List<Medication> Medications { get; set; } = new List<Medication>();
        public List<PatientAssessment> Assessments { get; set; } = new List<PatientAssessment>();
    }

    class Doctor : Person
    {
        public int Id { get; set; }
        public string Specialty { get; set; }
        public List<Patient> Patients { get; set; } = new List<Patient>();
        public string Username { get; set; }

        public void PrescribeDiagnosis(Patient patient, Diagnosis diagnosis)
        {
            patient.Diagnoses.Add(diagnosis);
        }
    }

    class ChiefDoctor : Doctor
    {
        public List<Diagnosis> PossibleDiagnoses { get; set; } = new List<Diagnosis>();

        public void AddPossibleDiagnosis(Diagnosis diagnosis)
        {
            diagnosis.Id = ++lastDiagnosisId;
            PossibleDiagnoses.Add(diagnosis);
        }

        public void ViewPossibleDiagnoses()
        {
            Console.WriteLine("Possible Diagnoses:");
            foreach (var diagnosis in PossibleDiagnoses)
            {
                Console.WriteLine($"{diagnosis.Id}. {diagnosis.Name} - Diagnosed by {diagnosis.DiagnosingDoctor.Name} on {diagnosis.DiagnosisTime}");
            }
        }
    }

    class Diagnosis
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Doctor DiagnosingDoctor { get; set; }
        public DateTime DiagnosisTime { get; set; }
    }

    class Medication
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Dosage { get; set; }
        public DateTime EndDate { get; set; }
    }

    class PatientAssessment
    {
        public Patient Patient { get; set; }
        public DateTime AssessmentDate { get; set; }
        public string AssessmentNotes { get; set; }
        public int HeartRate { get; set; }
        public int BloodPressure { get; set; }
    }

    static void Main(string[] args)
    {
        users.Add(new User { Username = "doctor1", Password = "password1", Role = "Doctor" });
        users.Add(new User { Username = "doctor2", Password = "password2", Role = "Doctor" });
        users.Add(new User { Username = "chiefdoctor", Password = "chiefpass", Role = "ChiefDoctor" });
        users.Add(new User { Username = "admin", Password = "adminpass", Role = "Admin" });

        var doctor1 = new Doctor { Id = ++lastDoctorId, Name = "Dr. Smith", Specialty = "General Practitioner", Username = "doctor1" };
        var doctor2 = new Doctor { Id = ++lastDoctorId, Name = "Dr. Johnson", Specialty = "Surgeon", Username = "doctor2" };

        chiefDoctor.AddPossibleDiagnosis(new Diagnosis { Id = ++lastDiagnosisId, Name = "Flu", DiagnosingDoctor = chiefDoctor, DiagnosisTime = DateTime.Now });
        chiefDoctor.AddPossibleDiagnosis(new Diagnosis { Id = ++lastDiagnosisId, Name = "Common Cold", DiagnosingDoctor = chiefDoctor, DiagnosisTime = DateTime.Now });

        var patient1 = new Patient
        {
            Id = ++lastPatientId,
            Name = "Patient1",
            DateOfBirth = new DateTime(1980, 1, 15),
            Gender = "Male",
            ContactInfo = "Phone: 123-456-7890",
        };

        var patient2 = new Patient
        {
            Id = ++lastPatientId,
            Name = "Patient2",
            DateOfBirth = new DateTime(1995, 5, 25),
            Gender = "Female",
            ContactInfo = "Phone: 987-654-3210",
        };

        doctors.Add(doctor1);
        doctors.Add(doctor2);
        doctors.Add(chiefDoctor);
        patients.Add(patient1);
        patients.Add(patient2);

        LoadData();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Welcome to the Medical Records System.");
            Console.WriteLine("Please log in.");

            Console.Write("Username: ");
            string username = Console.ReadLine();

            Console.Write("Password: ");
            string password = Console.ReadLine();

            authenticatedUser = AuthenticateUser(username, password);

            if (authenticatedUser != null)
            {
                Console.WriteLine($"Logged in as {authenticatedUser.Role}.");

                if (authenticatedUser.Role == "Admin")
                {
                    AdminMenu();
                    SaveData();
                }
                else if (authenticatedUser.Role == "Doctor")
                {
                    var doctor = doctors.FirstOrDefault(d => d.Username == authenticatedUser.Username);
                    if (doctor != null)
                    {
                        DoctorMenu(doctor);
                        SaveData();
                    }
                    else
                    {
                        Console.WriteLine("Doctor not found for the authenticated user.");
                    }
                }
                else if (authenticatedUser.Role == "ChiefDoctor")
                {
                    ChiefDoctorMenu(chiefDoctor);
                    SaveData();
                }
                else
                {
                    Console.WriteLine("Access denied. You are not authorized to use this system.");
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("Invalid username or password. Please try again.");
                Console.ReadLine();
            }
        }
    }

    static User AuthenticateUser(string username, string password)
    {
        return users.FirstOrDefault(user => user.Username == username && user.Password == password);
    }

    static void AdminMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Admin Menu:");
            Console.WriteLine("1. Add Doctor");
            Console.WriteLine("2. Add Patient");
            Console.WriteLine("3. View Patient Information");
            Console.WriteLine("4. Add Diagnosis");
            Console.WriteLine("5. Edit Patient Information");
            Console.WriteLine("6. Add Medication");
            Console.WriteLine("7. View Possible Diagnoses (Chief Doctor)");
            Console.WriteLine("8. Admin View All Patient Data");
            Console.WriteLine("9. Assign Doctor to Patient");
            Console.WriteLine("0. Switch Account or Exit"); 

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddDoctor();
                    break;
                case "2":
                    AddPatient();
                    break;
                case "3":
                    ViewPatientInfo();
                    break;
                case "4":
                    AddDiagnosis();
                    break;
                case "5":
                    EditPatientInfo();
                    break;
                case "6":
                    AddMedication();
                    break;
                case "7":
                    if (authenticatedUser.Role == "ChiefDoctor")
                        chiefDoctor.ViewPossibleDiagnoses();
                    else
                        Console.WriteLine("Access denied. You are not the Chief Doctor.");
                    break;
                case "8":
                    AdminViewAllPatientData();
                    break;
                case "9":
                    AssignDoctorToPatient();
                    break;
                case "0":
                    Console.WriteLine("0. Switch Account or Exit:");
                    Console.WriteLine("1. Switch Account"); 
                    Console.WriteLine("2. Exit"); 

                    string switchOrExitChoice = Console.ReadLine();
                    switch (switchOrExitChoice)
                    {
                        case "1":
                            return;
                        case "2":
                            Environment.Exit(0);
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Please select again.");
                            break;
                    }
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please select again.");
                    break;
            }
        }
    }



    static void DoctorMenu(Doctor doctor)
    {
        if (authenticatedUser.Role != "Doctor")
        {
            Console.WriteLine("Access denied. You are not authorized to use this menu.");
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
            return;
        }

        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Doctor Menu - Dr. {doctor.Name} ({doctor.Specialty}):");
            Console.WriteLine("1. View Patient Information");
            Console.WriteLine("2. Add Diagnosis");
            Console.WriteLine("3. Edit Patient Information");
            Console.WriteLine("4. Add Medication");
            Console.WriteLine("0. Switch Account or Exit"); 

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ViewPatientInfo();
                    break;
                case "2":
                    AddDiagnosis();
                    break;
                case "3":
                    EditPatientInfo();
                    break;
                case "4":
                    AddMedication();
                    break;
                case "0":
                    Console.WriteLine("0. Switch Account or Exit:");
                    Console.WriteLine("1. Switch Account"); 
                    Console.WriteLine("2. Exit");

                    string switchOrExitChoice = Console.ReadLine();
                    switch (switchOrExitChoice)
                    {
                        case "1":
                            return;
                        case "2":
                            Environment.Exit(0);
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Please select again.");
                            break;
                    }
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please select again.");
                    break;
            }
        }
    }



    static void ChiefDoctorMenu(ChiefDoctor chiefDoctor)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Chief Doctor Menu - Dr. {chiefDoctor.Name} ({chiefDoctor.Specialty}):");
            Console.WriteLine("1. View Patient Information");
            Console.WriteLine("2. Add Possible Diagnosis");
            Console.WriteLine("3. Edit Patient Information");
            Console.WriteLine("4. Add Medication");
            Console.WriteLine("5. View Possible Diagnoses");
            Console.WriteLine("6. Exit");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ViewPatientInfo();
                    break;
                case "2":
                    if (authenticatedUser.Role == "ChiefDoctor")
                    {
                        Console.Write("Enter new diagnosis name: ");
                        string newDiagnosisName = Console.ReadLine();

                        Diagnosis newDiagnosis = new Diagnosis
                        {
                            Name = newDiagnosisName,
                            DiagnosingDoctor = chiefDoctor,
                            DiagnosisTime = DateTime.Now
                        };
                        chiefDoctor.AddPossibleDiagnosis(newDiagnosis);

                        SaveData();

                        Console.WriteLine($"Diagnosis '{newDiagnosisName}' added by Chief Doctor {chiefDoctor.Name}.");
                    }
                    else
                    {
                        Console.WriteLine("Access denied. Only the Chief Doctor can add possible diagnoses.");
                    }
                    break;

                case "3":
                    EditPatientInfo();
                    break;
                case "4":
                    AddMedication();
                    break;
                case "5":
                    chiefDoctor.ViewPossibleDiagnoses();
                    break;
                case "0":
                    Console.WriteLine("0. Switch Account or Exit:");
                    Console.WriteLine("1. Switch Account"); 
                    Console.WriteLine("2. Exit");

                    string switchOrExitChoice = Console.ReadLine();
                    switch (switchOrExitChoice)
                    {
                        case "1":
                            return;
                        case "2":
                            Environment.Exit(0);
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Please select again.");
                            break;
                    }
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please select again.");
                    break;
            }
        }
    }




    static void AddDoctor()
    {
        Console.WriteLine("Enter doctor's name: ");
        string doctorName = Console.ReadLine();

        Console.WriteLine("Enter doctor's specialty: ");
        string specialty = Console.ReadLine();

        var doctor = new Doctor
        {
            Name = doctorName,
            Specialty = specialty
        };

        doctors.Add(doctor);

        Console.WriteLine($"Doctor {doctor.Name} added successfully.");
        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }

    static void AddPatient()
    {
        Console.WriteLine("Enter patient's name: ");
        string patientName = Console.ReadLine();

        Console.WriteLine("Enter patient's date of birth (yyyy-MM-dd): ");
        if (DateTime.TryParse(Console.ReadLine(), out DateTime dateOfBirth))
        {
            Console.WriteLine("Enter patient's gender: ");
            string gender = Console.ReadLine();

            Console.WriteLine("Enter patient's contact info: ");
            string contactInfo = Console.ReadLine();

            var patient = new Patient
            {
                Name = patientName,
                DateOfBirth = dateOfBirth,
                Gender = gender,
                ContactInfo = contactInfo
            };

            patients.Add(patient);

            Console.WriteLine($"Patient {patient.Name} added successfully.");
        }
        else
        {
            Console.WriteLine("Invalid date format.");
        }

        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }

    static void ViewPatientInfo()
    {
        Console.WriteLine("Enter patient name: ");
        string patientName = Console.ReadLine();

        var patient = patients.OfType<Patient>().FirstOrDefault(p => p.Name == patientName);

        if (patient != null)
        {
            Console.WriteLine($"Patient Information for {patient.Name}:");
            Console.WriteLine($"Date of Birth: {patient.DateOfBirth}");
            Console.WriteLine($"Gender: {patient.Gender}");
            Console.WriteLine($"Contact Info: {patient.ContactInfo}");

            if (patient.AttendingDoctor != null)
            {
                Console.WriteLine($"Attending Doctor: Dr. {patient.AttendingDoctor.Name} ({patient.AttendingDoctor.Specialty})");
            }

            Console.WriteLine("Diagnoses:");
            foreach (var diagnosis in patient.Diagnoses)
            {
                Console.WriteLine($"- {diagnosis.Name} (Diagnosed by Dr. {diagnosis.DiagnosingDoctor.Name} on {diagnosis.DiagnosisTime})");
            }

            Console.WriteLine("Medications:");
            foreach (var medication in patient.Medications)
            {
                Console.WriteLine($"- {medication.Name} ({medication.Dosage}) - End Date: {medication.EndDate}");
            }
        }
        else
        {
            Console.WriteLine("Patient not found.");
        }

        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }


    static void AddDiagnosis()
    {
        Console.WriteLine("Enter patient name: ");
        string patientName = Console.ReadLine();

        var patient = patients.OfType<Patient>().FirstOrDefault(p => p.Name == patientName);

        if (patient != null)
        {
            if (patient.AttendingDoctor != null)
            {
                Console.WriteLine($"Adding Diagnosis for {patient.Name}:");

                if (authenticatedUser.Role == "Doctor")
                {
                    Console.WriteLine("Available Diagnoses:");
                    foreach (var diagnosis in chiefDoctor.PossibleDiagnoses)
                    {
                        Console.WriteLine($"- {diagnosis.Name}");
                    }

                    Console.Write("Enter diagnosis name: ");
                    string diagnosisName = Console.ReadLine();

                    var possibleDiagnosis = chiefDoctor.PossibleDiagnoses.FirstOrDefault(diagnosis => diagnosis.Name == diagnosisName);

                    if (possibleDiagnosis != null)
                    {
                        var doctor = doctors.FirstOrDefault(d => d.Username == authenticatedUser.Username);
                        doctor.PrescribeDiagnosis(patient, possibleDiagnosis);

                        Console.WriteLine($"Diagnosis '{diagnosisName}' added successfully by Dr. {doctor.Name}.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid diagnosis name.");
                    }
                }
                else
                {
                    Console.WriteLine("Access denied. Only doctors can add diagnoses.");
                }
            }
            else
            {
                Console.WriteLine("Patient must have an assigned doctor to add a diagnosis.");
            }
        }
        else
        {
            Console.WriteLine("Patient not found.");
        }

        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }



    static void EditPatientInfo()
    {
        Console.WriteLine("Enter patient name: ");
        string patientName = Console.ReadLine();

        var patient = patients.OfType<Patient>().FirstOrDefault(p => p.Name == patientName);

        if (patient != null)
        {
            Console.WriteLine($"Editing Patient Information for {patient.Name}:");
            Console.Write("Enter new contact info: ");
            string newContactInfo = Console.ReadLine();

            patient.ContactInfo = newContactInfo;

            Console.WriteLine("Patient Information updated successfully.");
        }
        else
        {
            Console.WriteLine("Patient not found.");
        }

        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }

    static void AddMedication()
    {
        Console.WriteLine("Enter patient name: ");
        string patientName = Console.ReadLine();

        var patient = patients.OfType<Patient>().FirstOrDefault(p => p.Name == patientName);

        if (patient != null)
        {
            Console.WriteLine($"Adding Medication for {patient.Name}:");
            Console.Write("Enter medication name: ");
            string medicationName = Console.ReadLine();

            Console.Write("Enter dosage: ");
            string dosage = Console.ReadLine();

            Console.Write("Enter end date (yyyy-MM-dd): ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime endDate))
            {
                var medication = new Medication
                {
                    Name = medicationName,
                    Dosage = dosage,
                    EndDate = endDate
                };

                patient.Medications.Add(medication);

                Console.WriteLine($"Medication '{medicationName}' added successfully.");
            }
            else
            {
                Console.WriteLine("Invalid date format.");
            }
        }
        else
        {
            Console.WriteLine("Patient not found.");
        }

        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }


    static void AdminViewAllPatientData()
    {
        Console.WriteLine("Enter patient name: ");
        string patientName = Console.ReadLine();

        var patient = patients.OfType<Patient>().FirstOrDefault(p => p.Name == patientName);

        if (patient != null)
        {
            Console.WriteLine($"Admin Viewing All Data for {patient.Name}:");

            Console.WriteLine($"Date of Birth: {patient.DateOfBirth}");
            Console.WriteLine($"Gender: {patient.Gender}");
            Console.WriteLine($"Contact Info: {patient.ContactInfo}");

            Console.WriteLine("Attending Doctor:");
            if (patient.AttendingDoctor != null)
            {
                Console.WriteLine($"- Dr. {patient.AttendingDoctor.Name} ({patient.AttendingDoctor.Specialty})");
            }

            Console.WriteLine("Diagnoses:");
            foreach (var diagnosis in patient.Diagnoses)
            {
                Console.WriteLine($"- {diagnosis.Name} (Diagnosed by Dr. {diagnosis.DiagnosingDoctor.Name} on {diagnosis.DiagnosisTime})");
            }

            Console.WriteLine("Medications:");
            foreach (var medication in patient.Medications)
            {
                Console.WriteLine($"- {medication.Name} ({medication.Dosage}) - End Date: {medication.EndDate}");
            }
        }
        else
        {
            Console.WriteLine("Patient not found.");
        }

        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }

    static void AssignDoctorToPatient()
    {
        Console.WriteLine("Enter patient name: ");
        string patientName = Console.ReadLine();

        var patient = patients.OfType<Patient>().FirstOrDefault(p => p.Name == patientName);

        if (patient != null)
        {
            Console.WriteLine("Enter doctor name: ");
            string doctorName = Console.ReadLine();

            var doctor = doctors.OfType<Doctor>().FirstOrDefault(d => d.Name == doctorName);

            if (doctor != null)
            {
                Console.WriteLine($"What would you like to do?");
                Console.WriteLine("1. Replace the current attending doctor.");
                Console.WriteLine("2. Add an additional attending doctor.");
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    patient.AttendingDoctor = doctor;
                    Console.WriteLine($"Dr. {doctor.Name} is now the sole attending doctor for {patient.Name}.");
                }
                else if (choice == "2")
                {
                    if (patient.AttendingDoctor == null)
                    {
                        patient.AttendingDoctor = doctor;
                        Console.WriteLine($"Dr. {doctor.Name} is now the attending doctor for {patient.Name}.");
                    }
                    else
                    {
                        var existingDoctor = patient.AttendingDoctor;
                        patient.AttendingDoctor = new Doctor
                        {
                            Name = $"{existingDoctor.Name}, Dr. {doctor.Name}",
                            Specialty = $"{existingDoctor.Specialty}, {doctor.Specialty}"
                        };
                        Console.WriteLine($"Dr. {doctor.Name} is now an additional attending doctor for {patient.Name}.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid choice.");
                }
            }
            else
            {
                Console.WriteLine("Doctor not found.");
            }
        }
        else
        {
            Console.WriteLine("Patient not found.");
        }

        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }



    static void SaveData()
    {
        using (StreamWriter writer = new StreamWriter("data.txt"))
        {
            foreach (var user in users)
            {
                writer.WriteLine($"{user.Username},{user.Password},{user.Role}");
            }

            foreach (var doctor in doctors)
            {
                writer.WriteLine($"Doctor,{doctor.Name},{doctor.Specialty}");
            }

            foreach (var patient in patients)
            {
                writer.WriteLine($"Patient,{patient.Name},{patient.DateOfBirth},{patient.Gender},{patient.ContactInfo}");

                foreach (var diagnosis in patient.Diagnoses)
                {
                    writer.WriteLine($"Diagnosis,{diagnosis.Name},{diagnosis.DiagnosingDoctor.Name},{diagnosis.DiagnosisTime}");
                }

                foreach (var medication in patient.Medications)
                {
                    writer.WriteLine($"Medication,{medication.Name},{medication.Dosage},{medication.EndDate}");
                }
            }
        }

        Console.WriteLine("Data saved successfully.");
    }



    static void LoadData()
    {
        if (File.Exists("data.txt"))
        {
            using (StreamReader reader = new StreamReader("data.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(',');

                    if (parts.Length > 0)
                    {
                        string type = parts[0];

                        if (type == "Doctor" && parts.Length == 3)
                        {
                            var doctor = new Doctor
                            {
                                Name = parts[1],
                                Specialty = parts[2]
                            };
                            doctors.Add(doctor);
                        }
                        else if (type == "Patient" && parts.Length == 5)
                        {
                            var patient = new Patient
                            {
                                Name = parts[1],
                                DateOfBirth = DateTime.Parse(parts[2]),
                                Gender = parts[3],
                                ContactInfo = parts[4]
                            };
                            patients.Add(patient);
                        }
                        else if (type == "User" && parts.Length == 4)
                        {
                            var user = new User
                            {
                                Username = parts[1],
                                Password = parts[2],
                                Role = parts[3]
                            };
                            users.Add(user);
                        }
                        else if (type == "Diagnosis" && parts.Length == 4)
                        {
                            var diagnosis = new Diagnosis
                            {
                                Name = parts[1],
                                DiagnosingDoctor = doctors.FirstOrDefault(d => d.Name == parts[2]),
                                DiagnosisTime = DateTime.Parse(parts[3])
                            };
                            var patient = patients.OfType<Patient>().FirstOrDefault(p => p.Name == parts[1]);
                            if (patient != null)
                            {
                                patient.Diagnoses.Add(diagnosis);
                            }
                        }
                    }
                }

                Console.WriteLine("Data loaded successfully.");
            }
        }
        else
        {
            Console.WriteLine("Data file not found. Starting with empty data.");
        }
    }
}