# OnlyBooksMicroservices
Il progetto si presenta come un Back End di un eventuale sito che gestisce dei Libri. Ci sono due ruoli: Admin [0] e User [1]. L'admin è un Autore, che avrà accesso ad alcune API che ovviamente non saranno accessibili da User. Quest'ultimo invece è inteso come un potenziale acquirente dei libri. 
L'architettura è a Microservizi, quindi con due applicativi distinti, che comunicano debolmente tra loro:
- AuthService: Il primo microservizio è quello che gestisce l'Autenticazione. Con essa, tutto ciò che ne deriva, quindi: Registrazione, Login e JWT (JSON Web Token);
- BooksManagementService: Il secondo è il microservizio che gestisce i libri. L'autore potrà inserire un nuovo libro nel database, con relative informazioni, potrà modificarlo (solo se appartiene a lui il libro) e potrà cancellarlo. Lo User potrà vedere che libri ci sono nel database, e fare alcune read personalizzate come: GetAllBooks, ReadBookById, GetAllByAuthor e GetAllByGenre. 
- Il database è Microsoft SQL Server (relazionale).

Il secondo microservizio (BooksManagement) chiama il primo (Auth) per ottenere informazioni che sono contenute nei Claims del JSON Web Token (JWT), come l'ID e lo Usertype (Admin o User) di chi è loggato al momento; 

Il Database ha tre tabelle: Users (email, password criptata e usertype), UserDetails (dati anagrafici) e Books (info inerenti ai singoli libri);
Essendo il database relazionale, c'è una relazione nella tabella Books, ossia su AuthorId, che è una OneToOne a UserDetails (l'anagrafica dell'utente), e c'è inoltre un'altra OneToOne in Users, sulla colonna UserDetailsId che punta alla PK di UserDetails;

Il genere del libro nell'applicativo è un Enum:
Autobiography = 0, 
Biography = 1, 
Fantasy = 2,
Fiction = 3,
Historical = 4,
Thriller = 5,
Romance = 6,
Science = 7,
Horror = 8

L'applicativo può funzionare in due modi:

- Da Visual Studio/Visual Studio Code, scaricando i due branch 'Auth' e 'BooksManagement', runnando entrambi da IDE. Prima di fare ciò, bisogna settare un DB SQL Server e modificare la relativa stringa di connessione nell'appsettings.json. Inoltre bisogna modificare, sempre nello stesso file nel BooksManagement, la sezione: AuthService, e rimpiazzarla con la seguente stringa: "http://localhost:5225/Auth/GetMyId"; Per testare le API, si può usare Swagger, che si aprirà da solo quando si avvieranno i due progetti da terminale con il comando: 
        dotnet watch run
È possibile anche testare le api con Postman, inserendo i relativi indirizzi (localhost:5255 per Auth e localhost:5077/api per BooksManagement);

- Con Docker e Kubernetes. Per farlo partire con queste tecnologie, ho scritto un'apposita guida, poichè non è così semplice ed immediato.

GUIDA CON DOCKER E KUBERNETES
1. Installare Docker Desktop
2. Installare WSL2 Linux Kernel Update package, riavvia Windows
3. Apri Docker Desktop, vai su Settings, poi su Kubernetes sulla barra di sinistra, attiva la checkbox su Enable Kubernetes	
4. Pullare i due microservizi dal terminale Visual Studio Code: 
        docker pull shadymanu/booksmanagementsrv 
        docker pull shadymanu/authsrv
(La mia repository con queste Immagini su DockerHub è la seguente: https://hub.docker.com/u/shadymanu )
5. Scaricare la cartella K8S dal seguente link (o da questa repository, andando direttamente sul branch Kubernetes): 
        https://github.com/ShadyManu/OnlyBooksMicroservices/tree/Kubernetes
6. Aprire Visual Studio Code, e posizionarsi da terminale dentro questa cartella K8S che contiene i 7 file con estensione .yaml
7. Dobbiamo deployare i vari file, con i seguenti comandi da terminale di Visual Studio Code (sempre trovandosi come percorso del    terminale in questa cartella K8S scaricata):
        kubectl apply -f auth-depl.yaml
        kubectl apply -f auth-np-srv.yaml  (Questa sarà la Node Port di accesso al microservizio Auth)
        kubectl apply -f booksmanagementsrv-depl.yaml
        kubectl apply -f booksmanagement-np-srv.yaml  (Questa sarà la Node Port di accesso al microservizio Books)
        kubectl apply -f ingress-srv.yaml (Facoltativa, serve se si vuole usare Nginx)
        kubectl apply -f local-pvc.yaml (Alloca 200 MegaByte di Persistent Volume Claim per SQL Server, si può modificare la grandezza           all'interno del file dalla voce: storage: 200Mi)
8. Creare una password per il server SQL: 
        kubectl create secret generic mssql --from-literal=SA_PASSWORD="pA55w0rD"
9. Digitare sempre dallo stesso terminale:
        kubectl apply -f mssql-onlybooks-depl.yaml
10. Assicurarsi sull’app DockerDesktop sia tutto in funzione, oppure da terminale:
        kubectl get services
        kubectl get deployments
        kubectl get pods
        
Adesso l’ambiente dovrebbe funzionare correttamente. Per testare le chiamate API, si può utilizzare Postman, utilizzando due metodi:
Le Node Ports, che si usano in genere per lo sviluppo, e con Nginx:

- Con le Node Ports:
1. Posizionarsi con il terminale di Visual Studio Code nella cartella K8S
2. Scrivere il seguente comando sul terminale:
        kubectl get services;
3. Localizzare i seguenti servizi avviati in precedenza, con il comando: kubectl get services
        authnpservice-srv --> PORT --> es. 80:32637/TCP;
        booksmanagementnpservice-srv --> PORT --> es. 80:31758/TCP;
4. Queste sono le Node Ports per accedere ai due servizi (Auth 32637 e Book 31758);
5. Da Postman, l’indirizzo per testare le API sarà quindi:
        http://localhost:32637/Auth/api  ->  es. http://localhost:32637/Auth/Register
        http://localhost:31758/api/Book/api -> es. http://localhost:31758/api/Book/InsertBook
        
- Con Nginx, è il seguente:
1. Aprire in C:\Windows\System32\drivers\etc il file: hosts (possibilmente con Visual Studio Code, oppure con un editor di testo);
2. Sopra la scritta # Added by Docker Desktop aggiungere la seguente stringa:
        127.0.0.1 onlyb.com
L’indirizzo deve coincidere con quello che nello stesso documento è assegnato al localhost. Es:
        # localhost name resolution is handled within DNS itself.
        #	127.0.0.1       localhost
3. Salvare il file. Se lo avete aperto con Visual Studio Code, vi darà errore e vi dirà di riprovare come amministratore. Fatelo, perché è un file che può essere modificato solamente come admin. Se siete con un editor di testo, avrete qualche problema in più: salvatelo magari sul desktop, togliete il vecchio file dalla cartella etc e riportate il nuovo file che avete salvato sul desktop (con lo stesso nome) dentro la cartella; quando salvate con editor di testo, selezionate sotto Salva Come: tutti i file;
4. A questo punto, potrete accedere alle API con Postman con l’indirizzo: http://onlyb.com
        Esempio: http://onlyb.com/Auth/Register 
        Esempio: http://onlyb.com/api/Book/InsertBook
        
Questo è possibile perché prima abbiamo avviato il file facoltativo ingress-srv.yaml . Se lo aprite con Visual Studio Code, potete vedere al suo interno la mappatura dei due microservizi, mappati con il loro Cluster IP interno a Kubernetes.
Ho preparato una lista di chiamate tramite Postman. Per usarle, dovete avere un account Postman, e seguire questo link:
https://www.postman.com/shadymanu/workspace/onlybooksgithub

Altrimenti nella cartella K8S cè un file JSON da importare sempre su Postman (Import e selezionate/droppate il file JSON)
Sia che si importa il file che ho preparato sia che si segue il link, fare attenzione a cambiare l’indirizzo con le proprie Node Ports come spiegato, poiché vengono generate in modo casuale, e le mie non corrisponderebbero alle vostre. Mentre se si sceglie il metodo Nginx, dovrebbe già coincidere tutto.

Per domande o consigli su come migliorare il progetto, potete contattarmi sulla mia email personale: manuelraso1994@gmail.com
Se ti fa piacere, seguimi su LinkedIn: https://www.linkedin.com/in/manuel-raso/
