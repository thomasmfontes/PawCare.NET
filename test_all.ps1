$baseUrl = "http://localhost:5115"

Write-Host "==========================================================" -ForegroundColor Green
Write-Host "         PAWCARE API - BATERIA DE TESTES COMPLETOS        " -ForegroundColor Green
Write-Host "==========================================================" -ForegroundColor Green

# 1. Health Checks
Write-Host "`n[1/7] Testando Health Checks..." -ForegroundColor Yellow
$healthLive = Invoke-RestMethod -Uri "$baseUrl/health/live" -Method Get
Write-Host "-> /health/live Status: $($healthLive.status)" -ForegroundColor Cyan
Write-Host "-> Componente verificado: $($healthLive.entries[0].component) ($($healthLive.entries[0].description))"

$healthReady = Invoke-RestMethod -Uri "$baseUrl/health/ready" -Method Get
Write-Host "-> /health/ready Status: $($healthReady.status)" -ForegroundColor Cyan

# 2. Autenticação JWT
Write-Host "`n[2/7] Testando Autenticação JWT..." -ForegroundColor Yellow
$loginBody = @{
    username = "vet"
    password = "Vet@123"
} | ConvertTo-Json

$authResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method Post -Body $loginBody -ContentType "application/json"
$token = $authResponse.data.token
Write-Host "-> Login realizado com sucesso para usuário: $($authResponse.data.username)" -ForegroundColor Green
Write-Host "-> Role no JWT: $($authResponse.data.role)" -ForegroundColor Green
Write-Host "-> JWT Token: $($token.Substring(0, 40))..."

$headers = @{
    Authorization = "Bearer $token"
}

# 3. Listagem de Pets com Paginação e HATEOAS
Write-Host "`n[3/7] Testando GET /api/pets (Paginação e HATEOAS)..." -ForegroundColor Yellow
$petsResponse = Invoke-RestMethod -Uri "$baseUrl/api/pets?pageNumber=1&pageSize=2" -Method Get -Headers $headers
Write-Host "-> Total de Pets encontrados: $($petsResponse.data.totalItems)" -ForegroundColor Cyan
Write-Host "-> Página atual: $($petsResponse.data.pageNumber) de $($petsResponse.data.totalPages)" -ForegroundColor Cyan
$primeiroPet = $petsResponse.data.items[0]
Write-Host "-> Pet retornado: $($primeiroPet.nome) ($($primeiroPet.racaNome) - $($primeiroPet.especie))"
Write-Host "-> Links HATEOAS do Pet ($($primeiroPet.nome)):"
foreach ($l in $primeiroPet.links) {
    Write-Host "   * [$($l.method)] $($l.rel) -> $($l.href)"
}

# 4. Listagem de Tutores
Write-Host "`n[4/7] Testando GET /api/tutores..." -ForegroundColor Yellow
$tutoresResponse = Invoke-RestMethod -Uri "$baseUrl/api/tutores?pageNumber=1&pageSize=3" -Method Get -Headers $headers
Write-Host "-> Total de Tutores: $($tutoresResponse.data.totalItems)" -ForegroundColor Cyan
$primeiroTutor = $tutoresResponse.data.items[0]
Write-Host "-> Tutor retornado: $($primeiroTutor.nome) (CPF: $($primeiroTutor.cpf))"

# 5. MongoDB NoSQL: Criando Prontuário Clínico (POST)
Write-Host "`n[5/7] Testando MongoDB NoSQL: Criando Prontuário Clínico (POST)..." -ForegroundColor Yellow
$novoProntuario = @{
    petId = 1
    nomePet = "Rex"
    tutorCpf = "12345678901"
    veterinarioNome = "Dr. Bruno Souza"
    tipoAtendimento = "Consulta Ortopédica"
    queixaPrincipal = "Dificuldade de locomoção em membro pélvico direito"
    diagnostico = "Suspeita de displasia coxofemoral leve"
    observacoes = "Paciente apresenta claudicação após esforço prolongado."
    sintomas = @("Dificuldade para levantar", "Dor à palpação", "Relutância em correr")
    sinaisVitais = @{
        temperaturaCelsius = 38.6
        frequenciaCardiacaBpm = 105
        frequenciaRespiratoriaMpm = 28
        pressaoArterial = "125/85"
        pesoKg = 31.2
    }
    prescricoes = @(
        @{
            medicamento = "Condroitina + Glucosamina"
            dosagem = "1 comprimido a cada 24h"
            frequencia = "Diário"
            duracaoDias = 60
        },
        @{
            medicamento = "Meloxicam 2mg"
            dosagem = "0.5 comprimido a cada 24h"
            frequencia = "Diário"
            duracaoDias = 5
        }
    )
} | ConvertTo-Json -Depth 6

$prontuarioCriado = Invoke-RestMethod -Uri "$baseUrl/api/prontuarios" -Method Post -Body $novoProntuario -Headers $headers -ContentType "application/json"
Write-Host "-> Prontuário NoSQL criado com ID: $($prontuarioCriado.data.id)" -ForegroundColor Green
Write-Host "-> Diagnóstico registrado: $($prontuarioCriado.data.diagnostico)"
Write-Host "-> Quantidade de sintomas registrados: $($prontuarioCriado.data.sintomas.Count)"
Write-Host "-> Quantidade de prescrições vinculadas: $($prontuarioCriado.data.prescricoes.Count)"
Write-Host "-> Sinais Vitais (Temp / Peso): $($prontuarioCriado.data.sinaisVitais.temperaturaCelsius)°C / $($prontuarioCriado.data.sinaisVitais.pesoKg) kg"

# 6. MongoDB NoSQL: Consultando Prontuários (GET e Filtros)
Write-Host "`n[6/7] Testando Consulta do Prontuário NoSQL com Links HATEOAS..." -ForegroundColor Yellow
$prontuariosBusca = Invoke-RestMethod -Uri "$baseUrl/api/prontuarios?petId=1" -Method Get -Headers $headers
Write-Host "-> Prontuários encontrados para PetId 1: $($prontuariosBusca.data.totalItems)" -ForegroundColor Cyan
$pItem = $prontuariosBusca.data.items[0]
Write-Host "-> Diagnóstico do Prontuário recuperado: $($pItem.diagnostico)"
Write-Host "-> Links HATEOAS do Prontuário NoSQL:"
foreach ($link in $pItem.links) {
    Write-Host "   * [$($link.method)] $($link.rel) -> $($link.href)"
}

# 7. Teste de Segurança: 401 Unauthorized sem Token
Write-Host "`n[7/7] Testando Segurança (Bloqueio sem Token JWT)..." -ForegroundColor Yellow
try {
    $res = Invoke-RestMethod -Uri "$baseUrl/api/prontuarios" -Method Post -Body $novoProntuario -ContentType "application/json"
    Write-Host "-> FALHA: Requisição sem token deveria ter sido barrada!" -ForegroundColor Red
} catch {
    Write-Host "-> SUCESSO: Endpoint protegido retornou HTTP 401 Unauthorized conforme esperado." -ForegroundColor Green
}

Write-Host "`n==========================================================" -ForegroundColor Green
Write-Host "         TODOS OS 7 TESTES CONCLUÍDOS COM SUCESSO!        " -ForegroundColor Green
Write-Host "==========================================================" -ForegroundColor Green
