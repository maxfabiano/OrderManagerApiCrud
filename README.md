Desenvolvedor: Max Fabiano Carlis Nunes da Silva
O projeto é composto por quatro partes principais:

DataBaseApiPedidos
OrderManagerBack
OrderManagerFront
OrderManagerUnityTest
Organização dos Projetos:

Backend:
Composto por dois projetos:
DataBaseApiPedidos
OrderManagerBack

Frontend:
Composto pelo projeto OrderManagerFront.
Testes Unitários:

O projeto OrderManagerUnityTest é dedicado aos testes unitários do OrderManagerBack.

Detalhes de Integração:
O Frontend pode ser executado separadamente do Backend, pois ele utiliza a API do backend para consumir os dados.

Como Rodar o Projeto:
Clonar o Repositório:
Baixe os arquivos clonando o repositório.

Abrir no Visual Studio:
Abra a solução diretamente no Visual Studio.

Configurar os Projetos de Inicialização:

Clique na seta ao lado do botão Iniciar no Visual Studio.
Escolha a opção Configurar Projetos de Inicialização.
Selecione a opção Vários Projetos de Inicialização e marque:
OrderManagerBack iniciar
OrderManagerFront iniciar

Somente estes dois projetos devem estar marcados para iniciar. OrderManagerBack OrderManagerFront

Executar o Projeto:
Após a configuração, clique no botão Iniciar para rodar o backend e o frontend simultaneamente.

