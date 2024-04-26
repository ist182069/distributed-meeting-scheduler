### [Design and Implementation of Distributed Applications](https://fenix.tecnico.ulisboa.pt/disciplinas/PADI/2021-2022/1-semestre)

## Introductory Note

This course is considered one of the hardest courses in the Department of Computer Science and Engineering by the students.

The course was taught by Professor Luís Rodrigues, and this project was evaluated by Professor João Garcia, who awarded it a final grade of 17.5/20. This project was the second highest grade at Instituto Superior Técnico for this course in the semester of 2019/2020.

The project idea was to create a meeting scheduling service. The interaction involved a client connecting to a server and issuing commands such as creating meetings scheduled for any future date. These meetings would be scheduled for a room in a given location (e.g., Lisbon; Porto) at a time chosen by the coordinator (the meeting creator process).

Client options included creating, joining, and closing meetings. If the closing of the meetings was successful, they would transition to the "SCHEDULED" state; otherwise, they would be canceled.

The objective of the servers, as per our interpretation of the project statement located in the *./Statement* directory, was for each server to use "Uniform Reliable Broadcast" in combination with "Atomic Registers" and "Distributed Mutual Exclusion" to ensure total order of messages broadcasted by the servers.

Future work would involve implementing "Message Queues" on each server to ensure that if an operation is not successfully broadcasted, it would be re-broadcasted at some later point in time.

It is with great pleasure that I share with any reader of this repository the code to aid in the creation of any distributed application in future projects.

## Language and Modules Used

This project was implemented in the C# language. Being a distributed application, the most used and imported module in Visual Studio was Remoting. Additionally, we also used JSON.NET from Newtonsoft to parse certain classes and send them over the network.

## Directories

- In the *./Client* directory, we naturally find the code for the client application that will make requests to the server application. This application is a C# console in which commands are issued to the servers. The structure of the main code follows a "Command" design pattern. In the *./Communications* directory, we find the Remoting functions necessary for the communications used by us.

- In the *./Library* directory, there are the libraries shared by the Client and Server applications.

- *./PCS* is the machine responsible for starting Client or Server processes remotely issued through the PuppetMaster application.

- *./PuppetMaster* is a Windows Form application that allows sending initialization commands to Clients or Servers and running scripts on them.

- *./Server* naturally is the directory where the server is located. In the *./Communications* directory, we find the Remoting functions necessary for the communications used by the server.

## Puppet Master Scripts

At the root of this directory, there is an example interaction showing how to run the project. The scripts to be run by the client are located in the *./Client/Scripts* directory. The same goes for the scripts to be run by the PuppetMaster; these are located in the *./PuppetMaster/Scripts* directory.

## Group 16

- 82069 - José Brás - MEIC-T (Final Grade: 18/20)
- 87527 - Duarte Nascimento - MEIC-T (Final Grade: 17/20)
- 87557 - Pedro Agostinho - METI (Final Grade: 17/20)
