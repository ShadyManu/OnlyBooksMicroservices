# OnlyBooksMicroservices
The project is presented as a Back End of a possible site that manages Books. There are two roles: Admin [0] and User [1]. The admin is an Author, who will have access to some APIs that obviously will not be accessible by User. The latter is intended as a potential purchaser of books. The architecture is at Microservices, therefore with two distinct applications, which weakly communicate with each other:

- AuthService: The first microservice is the one that handles Authentication and everything that comes with it, like: Registration, Login and JWT (JSON Web Token);
- BooksManagementService: The second is the microservice that manages books. The author can insert a new book in the database, with related information, can modify it (only if the book belongs to him) and can delete it. The User will be able to see which books are in the database, and make some customized reads such as: GetAllBooks, ReadBookById, GetAllByAuthor and GetAllByGenre.
- The database is Microsoft SQL Server (relational).

The second microservice (BooksManagement) calls the first one (Auth) to obtain information that is contained in the Claims of the JSON Web Token (JWT), like the ID and the Usertype (Admin or User) of who is currently logged in;

The Database has three tables: Users (email, crypted password and usertype), UserDetails (personal data) and Books (information about individual books); Being a relational database, there is a relationship in the table Books, on AuthorId column, which is a OneToOne a UserDetails (the user’s personal data), and there is also another OneToOne in Users, on the UserDetailsId column that points to the PK of UserDetails;

The genre of the book in the application is an Enum: Autobiography = 0, Biography = 1, Fantasy = 2, Fiction = 3, Historical = 4, Thriller = 5, Romance = 6, Science = 7, Horror = 8

The application can work in two ways:

- From Visual Studio/Visual Studio Code, by downloading the two branches 'Auth' and 'BooksManagement', running both from IDE. Before doing this, you have to set up an SQL Server DB and change connection string in the appsettings.json. Also you have to edit, always in the same file in BooksManagement, the section: AuthService, and replace it with the following string: "http://localhost:5225/Auth/GetMyId"; To test the APIs, you can use Swagger, which will open by itself when you start the two projects with the command: 
        dotnet watch run 
You can also test the APIs with Postman, by entering their addresses (localhost:5255 for Auth and localhost:5077/api for BooksManagement);

- With Docker and Kubernetes. To get it started with these technologies, I wrote a special guide, because it is not so simple and immediate.

GUIDE WITH DOCKER AND KUBERNETES

1. Install Docker Desktop
2. Install WSL2 Linux Kernel Update package, restart Windows
3. Open Docker Desktop, go to Settings, Select Kubernetes from the left sidebar, next to Enable Kubernetes, select the checkbox.
4. Pull the two microservices from the Visual Studio Code terminal: 
        docker pull shadymanu/booksmanagementsrv
        docker pull shadymanu/authsrv 
(My repository with these Images on DockerHub is as follows: https://hub.docker.com/u/shadymanu )
5. Download the K8S folder from the following link (or from this repository, going directly to the Kubernetes branch):                             https://github.com/ShadyManu/OnlyBooksMicroservices/tree/Kubernetes
6. Open Visual Studio Code, and navigate to this K8S folder containing the 7 files with the extension .yaml
7. We need to deploy the various files, with the following terminal commands of Visual Studio Code (always found as terminal path in this downloaded K8S folder): 
        kubectl apply -f auth-depl.yaml 
        kubectl apply -f auth-np-srv.yaml (This will be the Node Port access to the Auth microservice) 
        kubectl apply -f booksmanagementsrv-depl.yaml 
        kubectl apply -f booksmanagement-np-srv.yaml (This will be the Node Port access to the Books microservice) 
        kubectl -f ingress-srv.yaml apply (Optional, if you want to use Nginx) 
        kubectl apply -f local-pvc.yaml (Allocates 200 megabytes of Persistent Volume Claim for SQL Server, you can change the file size         from: storage: 200Mi)
8. Create a password for the SQL server: 
        kubectl create secret generic mssql --from-literal=SA_PASSWORD="pA55w0rD"
9. Always type from the same terminal: 
        kubectl apply -f mssql-onlybooks-depl.yaml
10. Make sure everything is running on the DockerDesktop app, or from the terminal with these commands: 
        kubectl get services 
        kubectl get deployments 
        kubectl get pods
        
The environment should now work properly. To test API calls, you can use Postman, using two methods:
Node Ports, which are usually used for development, and with Nginx:

- With the Node Ports:
1. Navigate to the Visual Studio Code terminal in the K8S folder
2. Write the following command on the terminal: 
        kubectl get services;
3. Locate the following previously started services with the command: 
        kubectl get services authnpservice-srv --> PORT --> e.g. 80:32637/TCP 
        booksmanagementservice-srv --> PORT --> e.g. 80:31758/TCP;
4. These are the Node Ports to access the two services (Auth 32637 and Book 31758);
5. From Postman, the address to test the API will be: 
        http://localhost:32637/Auth/api -> eg http://localhost:32637/Auth/Register 
        http://localhost:31758/api/Book/api -> e.g. http://localhost:31758/api/Book/InsertBook
        
- With Nginx, it is as follows:
1. Open in C:\Windows\System32\drivers\etc the file: hosts (possibly with Visual Studio Code, or with a text editor);
2 Above # Added by Docker Desktop add the following string: 
        127.0.0.1 onlyb.com 
The address must match the address assigned to the localhost in the same document. 
        Eg: # localhost name resolution is handled within DNS itself. 
        # 127.0.0.1 localhost
3. Save the file. If you opened it with Visual Studio Code, it will give you an error and tell you to try again as administrator. Do it, because it is a file that can only be edited as admin. If you are with a text editor, you will have some more problems: save it on the desktop, remove the old file from the folder etc and bring back the new file you saved on the desktop (with the same name) inside the folder; when you save with text editor, select under Save As: all files;
4. At this point, you can access the APIs with Postman with the address: http:///onlyb.com 
        Example: http://onlyb.com/Auth/Register 
        Example: http:///onlyb.com/api/Book/InsertBook 

This is possible because we started the optional file ingress-srv.yaml. If you open it with Visual Studio Code, you can see inside of it the mapping of the two microservices, mapped with their internal Cluster IP in Kubernetes. 
I’ve prepared a list of calls through Postman. To import them, open Postman, Click on Collections, Import, and paste the following link: https///api.postman.com/collections/26153867-26d5281c-211e-4aa2-9941-b5b9eea9dcc7?access_key=PMAT-01H1C362JNGNGDXA2XHQ8Y193H 
Otherwise in the K8S folder there is a JSON file to import to Postman. 
If you import the file I prepared (in both ways), be careful to change the address with your Node Ports as explained, as they are generated randomly, and mine would not match yours. While if you choose the Nginx method, it should already coincide all.

For any questions or advice on how to improve the project, you can contact me on my personal email: manuelraso1994@gmail.com 
If you want, follow me on LinkedIn: https///www.linkedin.com/in/manuel-raso
