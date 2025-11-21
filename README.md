# Simulador.Api

Aplicação Web API que simula recomendações de investimentos de acordo com comportamento financeiro do cliente.

Desenvolvida em .Net Core 8 e Sqlite 3 para o desafio CaixaVerso em Nov/2025.

Para executar a API via docker seguir os seguintes passos:
1. Baixe o zip do código ou clone o projeto disponibilizado neste repositório;
2. Descompacte o projeto;
3. Vá até a raiz da solução: 'cd <pasta onde está o Simulador.Api.sln>'
4. Execute o docker compose: 'docker compose up --build' ou em segundo plano 'docker compose up -d --build'
5. Abra o navegador em: http://localhost:8080/swagger
6. Para verificar os logs da APi: 'docker logs simulador-api -f'
7. Parar ou derrubar a API: 'docker compose down'
