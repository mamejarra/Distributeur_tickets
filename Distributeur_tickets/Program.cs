using System;
using System.Collections.Generic;
using System.IO;
class DistributeurTickets
{
    // Dictionnaire pour stocker les numéros actuels attribués pour chaque type d'opération
    private static Dictionary<string, int> numerosTickets = new Dictionary<string, int>()
    {
        { "V", 0 },  // V pour Versement
        { "R", 0 },  // R pour Retrait
        { "I", 0 }   // I pour Informations
    };

    // Chemin d'accès au fichier pour stocker les derniers numéros attribués
    private static string fichierNumeros = Path.Combine(Path.GetTempPath(), "fnumero.txt");
    private static string fichierClients = Path.Combine(Path.GetTempPath(), "clients.txt");

    // Liste pour stocker les informations des clients
    private static List<Client> listeClients = new List<Client>();

    // Méthode principale de l'application
    static void Main()
    {
        // Charger les derniers numéros attribués depuis le fichier et les clients existants
        ChargerNumeros();
        ChargerClients();

        // Boucle pour gérer la demande de numéros des clients
        while (true)
        {
            Console.WriteLine("Bienvenue au distributeur automatique de tickets.");
            Console.WriteLine("Quel type d'opération souhaitez-vous effectuer ?");
            Console.WriteLine("1 - Versement");
            Console.WriteLine("2 - Retrait");
            Console.WriteLine("3 - Informations");
            Console.WriteLine("4 - Quitter");
            Console.Write("Veuillez entrer le numéro correspondant à l'opération : ");

            // Récupération du choix de l'utilisateur
            string choix = Console.ReadLine();
            string typeOperation = "";

            // Utilisation d'un switch pour déterminer le type d'opération
            switch (choix)
            {
                case "1":
                    typeOperation = "V";  // Versement
                    break;
                case "2":
                    typeOperation = "R";  // Retrait
                    break;
                case "3":
                    typeOperation = "I";  // Informations
                    break;
                case "4":
                    Console.WriteLine("Merci d'avoir utilisé le distributeur de tickets. Au revoir !");
                    SauvegarderNumeros();
                    SauvegarderClients();
                    AfficherListeClients();
                    return;  // Quitter l'application
                default:
                    Console.WriteLine("Merci de choisir un nombre compris entre 1 et 3 pour effectuer une opération, tapez 4 pour quitter.");
                    continue;  // Recommencer la boucle en cas de choix invalide
            }

            // Gestion de la demande de tickets pour l'opération choisie
            bool genererNouveauTicket = true;
            while (genererNouveauTicket)
            {
                // Récupérer les informations du client avec vérification de l'unicité du numéro de compte
                Client client = CreerClient();

                // Attribution d'un numéro de ticket pour l'opération choisie
                int numeroAttribue = AttribuerNumero(typeOperation);
                int personnesEnAttente = numeroAttribue - 1;

                // Affichage du ticket et du nombre de personnes en attente pour ce type d'opération
                Console.WriteLine($"\nVotre numéro est : {typeOperation}-{numeroAttribue}");
                Console.WriteLine($"Il y a {personnesEnAttente} personne(s) qui attend(ent) avant vous.\n");

                // Ajout du client à la liste et sauvegarde dans le fichier
                client.NumeroTicket = $"{typeOperation}-{numeroAttribue}";
                listeClients.Add(client);

                // Demander si l'utilisateur souhaite effectuer une autre opération
                Console.Write("Souhaitez-vous prendre un autre numéro pour cette même opération ? (o/n) : ");
                string reponse = Console.ReadLine().ToLower();

                if (reponse == "o")
                {
                    genererNouveauTicket = true;
                }
                else if (reponse == "n")
                {
                    genererNouveauTicket = false;
                }
                else
                {
                    Console.WriteLine("Erreur : Veuillez saisir 'o' pour oui ou 'n' pour non.");
                }
            }
        }
    }

    // Méthode pour attribuer un numéro de ticket en fonction du type d'opération
    private static int AttribuerNumero(string typeOperation)
    {
        numerosTickets[typeOperation]++;
        return numerosTickets[typeOperation];
    }

    // Méthode pour charger les derniers numéros attribués depuis le fichier
    private static void ChargerNumeros()
    {
        if (File.Exists(fichierNumeros))
        {
            string[] lignes = File.ReadAllLines(fichierNumeros);
            foreach (string ligne in lignes)
            {
                string[] parties = ligne.Split('-');
                if (parties.Length == 2 && numerosTickets.ContainsKey(parties[0]))
                {
                    numerosTickets[parties[0]] = int.Parse(parties[1]);
                }
            }
        }
    }

    // Méthode pour sauvegarder les derniers numéros attribués dans le fichier
    private static void SauvegarderNumeros()
    {
        List<string> lignes = new List<string>();
        foreach (var kvp in numerosTickets)
        {
            lignes.Add($"{kvp.Key}-{kvp.Value}");
        }
        File.WriteAllLines(fichierNumeros, lignes);
    }

    // Méthode pour créer un client avec vérification de l'unicité du numéro de compte
    private static Client CreerClient()
    {
        string numeroCompte;
        do
        {
            Console.Write("Entrez le numéro de compte du client : ");
            numeroCompte = Console.ReadLine();
            if (!VerifierUniciteNumeroCompte(numeroCompte))
            {
                Console.WriteLine("Numéro de compte déjà utilisé. Veuillez entrer un numéro unique.");
            }
        } while (!VerifierUniciteNumeroCompte(numeroCompte));

        Console.Write("Entrez le nom du client : ");
        string nom = Console.ReadLine();

        Console.Write("Entrez le prénom du client : ");
        string prenom = Console.ReadLine();

        return new Client { NumeroCompte = numeroCompte, Nom = nom, Prenom = prenom };
    }

    // Méthode pour vérifier l'unicité du numéro de compte
    private static bool VerifierUniciteNumeroCompte(string numeroCompte)
    {
        foreach (var client in listeClients)
        {
            if (client.NumeroCompte == numeroCompte)
                return false;
        }
        return true;
    }

    // Charger les informations des clients depuis le fichier
    private static void ChargerClients()
    {
        if (File.Exists(fichierClients))
        {
            string[] lignes = File.ReadAllLines(fichierClients);
            foreach (var ligne in lignes)
            {
                var data = ligne.Split(';');
                if (data.Length == 4)
                {
                    listeClients.Add(new Client
                    {
                        NumeroCompte = data[0],
                        Nom = data[1],
                        Prenom = data[2],
                        NumeroTicket = data[3]
                    });
                }
            }
        }
    }

    // Sauvegarder la liste des clients dans le fichier
    private static void SauvegarderClients()
    {
        var lignes = new List<string>();
        foreach (var client in listeClients)
        {
            lignes.Add($"{client.NumeroCompte};{client.Nom};{client.Prenom};{client.NumeroTicket}");
        }
        File.WriteAllLines(fichierClients, lignes);
    }

    // Méthode pour afficher la liste finale des clients
    private static void AfficherListeClients()
    {
        Console.WriteLine("\nListe des clients servis :");
        foreach (var client in listeClients)
        {
            Console.WriteLine($"Compte : {client.NumeroCompte}, Nom : {client.Nom}, Prénom : {client.Prenom}, Ticket : {client.NumeroTicket}");
        }
    }

    // Classe pour représenter un client
    class Client
    {
        public string NumeroCompte { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string NumeroTicket { get; set; }
    }
}