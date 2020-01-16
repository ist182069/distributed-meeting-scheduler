# Desenvolvimento de Aplicações Distribuídas

## Nota Introdutória
Este projecto consistiu em fazer um serviço de agendamento de reuniões.

A ideia era um cliente ligar-se a um servidor e emitir comandos como a criação de reuniões que seriam agendadas para qualquer futuro dia. Estas reuniões seriam agendadas para uma sala numa dada localização (i.e. Lisboa; Porto) numa hora escolhida pelo coordenador (processo criador da reunião). 

As opções realizadas pelos clientes incluíam poder criar, juntar-se e fechar reuniões. Se o fecho das reuniões fosse bem sucedido estas passariam ao estado "SCHEDULED", caso contrário estas seriam canceladas.

O objectivo dos servidores, e segundo a nossa interpretação do enunciado do projecto, que se encontra no directório "/Statement", foi cada servidor usar "Uniform Reliable Broadcast" em combinação com "Atomic Registers" e "Distributed Mutal Exclusion" de forma a garantir a ordem total das mensagens difundidas pelos servidores.

Trabalho futuro consistiria em implementar "Message Queues" em cada servidor de forma a garantir que se uma operação não for difundida com sucesso estas voltaria a sê-lo algures no tempo mais tarde.

A nota final atribuída a este projecto foi 17.5. Este projecto foi a segunda nota mais alta no Instituto Superior Técnico a esta unidade curricular no semestre de 2019/2020.

É com muito prazer que partilho com qualquer leitor deste repositório o código para ajudar na criação de qualquer aplicação distribuída em projectos futuros.

## Línguagem e Módulos Utilizados

Este projecto foi realizado na linguagem C#. Sendo este projecto uma aplicação distribuda o módulo mais utilizado e importado no Visual Studio foi o Remoting. Adicionalmente fizemos uso também do JSON.NET da Newtonsoft para parsar certas classes e enviá-las na rede.

## Directórios

- No directório */Client* encontramos naturalmente o código da aplicação cliente que fará os pedidos à aplicação servidor. Esta aplicação é uma consola C# na qual são emitidos comandos para os servidores. A estructura da "main" código segue um padrão de desenho "Command". No directório */Communications* encontram-se as funções de Remoting necessárias às comunicações usadas por nós.

- No directório */Library* encontram-se as bibliotecas partilhadas pela aplicação Cliente e Servidor.

- */PCS* é a máquina responsável por iniciar processos Cliente ou Servidor remotamente emitidos através da aplicação PuppetMaster.

- */PuppetMaster* é uma aplicação de Windows Form que permite enviar comandos de inicialização de Clientes ou Servidores e permite correr scripts nos mesmos.

- */Server* naturalmente é o directório onde se encontra o servidor. No directório */Communications* encontram-se as funções de Remoting necessárias às comunicações usadas pelo servidor.

## Enunciado

Eventualmente tambm colocarei o enúnciado do projecto.

## Scripts Puppet Master

Eventualmente colocarei aqui uma pasta com scripts para serem corridos no PuppetMaster ou em qualquer uma das máquinas Cliente ou Servidor.

## Grupo 16 
- 82069 - José Brás         -  MEIC-T  
- 87527 - Duarte Nascimento -  MEIC-T
- 87557 - Pedro Agostinho   -  METI
