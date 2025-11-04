AdedonhaAPI
Um repositório de palavras para o jogo Adedonha (Stop!). Este projeto é uma API RESTful robusta construída em .NET 8, projetada para servir como o back-end de um site de consulta de palavras.

🚀 Sobre o Projeto
O objetivo desta API é ser o maior e mais rápido catálogo de palavras para Adedonha. O site de front-end (consumidor desta API) permitirá que os usuários naveguem por categorias e encontrem palavras para se saírem bem no jogo.

A aplicação é dividida em dois módulos principais:

Módulo de Catálogo (Público):

Apresenta um "mural" com todas as categorias disponíveis (ex: Frutas, Carros, Animais).

Cada categoria exibe um card com 10 palavras aleatórias daquela categoria, atualizadas a cada visita.

O usuário pode clicar em uma categoria para abrir uma visualização detalhada.

Nesta visualização, o usuário tem acesso a uma tabela paginada e pesquisável com todas as palavras daquela categoria.

Módulo de Admin (Seguro):

Uma área de back-office protegida por autenticação JWT e Roles.

Fornece operações CRUD (Criar, Ler, Atualizar, Deletar) completas para Categorias e Palavras.

Inclui um endpoint para upload em massa de palavras através de planilhas.

🏛️ Arquitetura: Vertical Slice com Padrão REPR
Este projeto não utiliza a arquitetura N-Layer (em camadas) tradicional ou Controllers MVC. Em vez disso, adotamos a Arquitetura de Fatias Verticais (Vertical Slice Architecture - VSA).

O que é Vertical Slice?
Na VSA, organizamos nosso código em torno de features (funcionalidades), e não em torno de camadas técnicas (como Services, Repositories, Controllers).

Cada "fatia" vertical representa um único caso de uso ou feature e contém toda a lógica necessária para essa operação, de ponta a ponta. Isso resulta em um código com alta coesão (tudo o que é necessário para uma feature está junto) e baixo acoplamento (uma feature não depende de outra).

O Padrão REPR (Request-Endpoint-Response)
Para implementar a VSA em nossa API, utilizamos o padrão REPR (Requisição-Ponto de Extremidade-Resposta). Este padrão substitui os Controllers inchados e com múltiplas dependências por Endpoints focados em uma única ação.

Cada feature é composta por três componentes principais:

Request (Requisição): Um DTO (ou record) que modela a solicitação de entrada. Em um padrão CQRS, isso seria um Query (para leituras) ou Command (para escritas).

Endpoint (Ponto de Extremidade): Uma classe simples que define a rota, o verbo HTTP e a delegação da lógica. Usamos a biblioteca Carter para registrar esses endpoints de forma limpa, mantendo nosso Program.cs enxuto.

Response (Resposta): Um DTO que modela a resposta de saída.

A lógica de negócios em si é implementada em um Handler (usando MediatR), que é injetado no Endpoint. Isso torna cada feature isolada, fácil de encontrar e incrivelmente fácil de testar.
