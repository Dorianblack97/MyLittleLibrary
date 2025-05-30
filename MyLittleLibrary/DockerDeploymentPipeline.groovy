pipeline {
    agent any
    
    parameters {
        string(name: 'GIT_REPO_URL', defaultValue: 'https://github.com/your-username/your-blazor-repo.git', description: 'URL del repository Git')
        string(name: 'GIT_BRANCH', defaultValue: 'main', description: 'Branch da utilizzare')
        string(name: 'CONTAINER_NAME', defaultValue: 'blazor-server', description: 'Nome del container Docker')
        string(name: 'IMAGE_NAME', defaultValue: 'blazor-server-app', description: 'Nome dell\'immagine Docker')
        string(name: 'IMAGE_TAG', defaultValue: "${BUILD_NUMBER}", description: 'Tag dell\'immagine Docker')
        string(name: 'EXPOSED_PORT', defaultValue: '5000', description: 'Porta da esporre')
        string(name: 'DOCKER_NETWORK', defaultValue: 'bridge', description: 'Rete Docker da utilizzare')
    }
    triggers {
        // Polling SCM ogni 5 minuti, ma solo per il branch master
        pollSCM {
            scm('H/5 * * * *')
            branches('master')  // o 'main' se usi la nuova nomenclatura
        }
    
    environment {
        MONGODB_CREDENTIALS = credentials('mongodb-credentials')
        GIT_CREDENTIALS = credentials('git-credentials')
        FULL_IMAGE_NAME = "${params.IMAGE_NAME}:${params.IMAGE_TAG}"
		GIT_REPO_URL = 'git@github.com:username/repository.git'
    }
    
    stages {
        stage('Checkout') {
            steps {
                // Assicurati di fare checkout solo dal branch master
                checkout([$class: 'GitSCM',
                    branches: [[name: '*/master']],
                    userRemoteConfigs: [[url: 'https://your-git-repo-url.git']]
                ])
                
                // Opzionale: visualizza lo stato corrente del repository
                sh '''
                    git log -1 --pretty=format:"%h - %an, %ar : %s" HEAD
                    echo "Repository clonato con successo"
                '''
            }
        }
        
        stage('Build Docker Image') {
            steps {
                echo "Building Docker image ${FULL_IMAGE_NAME}..."
                
                // Costruzione dell'immagine Docker
                sh '''
                    # Cerca il Dockerfile nella directory corrente o in una sottodirectory BlazorServer
                    if [ -f "Dockerfile" ]; then
                        DOCKER_FILE_PATH="./Dockerfile"
                    elif [ -f "BlazorServer/Dockerfile" ]; then
                        DOCKER_FILE_PATH="./BlazorServer/Dockerfile"
                    else
                        echo "Dockerfile non trovato. Verifica la struttura del repository."
                        exit 1
                    fi
                    
                    echo "Usando Dockerfile in: $DOCKER_FILE_PATH"
                    
                    # Costruisci l'immagine Docker
                    docker build -t ${FULL_IMAGE_NAME} \
                    --build-arg ASPNETCORE_ENVIRONMENT=Production \
                    -f $DOCKER_FILE_PATH .
                '''
            }
        }
        
        stage('Deploy Container') {
            steps {
                echo "Deploying container ${params.CONTAINER_NAME}..."
                
                // Verifica se il container esiste già e lo rimuove
                sh '''
                    if docker ps -a | grep -q ${CONTAINER_NAME}; then
                        echo "Container ${CONTAINER_NAME} esiste. Aggiornamento in corso..."
                        docker stop ${CONTAINER_NAME}
                        docker rm ${CONTAINER_NAME}
                    else
                        echo "Container ${CONTAINER_NAME} non esiste. Creazione nuovo container..."
                    fi
                '''
                
                // Recupera l'indirizzo IP di MongoDB (assumendo che sia in un container chiamato 'mongodb')
                sh '''
                    MONGODB_HOST=$(docker inspect -f '{{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}' mongodb || echo "mongodb")
                    
                    # Ottieni la data di build per il tagging
                    BUILD_DATE=$(date +"%Y%m%d_%H%M%S")
                    
                    # Avvio del nuovo container
                    docker run -d \
                      --name ${CONTAINER_NAME} \
                      --network ${DOCKER_NETWORK} \
                      -p ${EXPOSED_PORT}:80 \
                      -e ASPNETCORE_ENVIRONMENT=Production \
                      -e MongoDB__ConnectionString=mongodb://${MONGODB_CREDENTIALS_USR}:${MONGODB_CREDENTIALS_PSW}@${MONGODB_HOST}:27017 \
                      -e MongoDB__DatabaseName=MyLittleLibrary \
                      -e BUILD_VERSION="${BUILD_NUMBER}" \
                      -e BUILD_DATE="${BUILD_DATE}" \
                      --restart unless-stopped \
                      ${FULL_IMAGE_NAME}
                    
                    # Taggare l'immagine con latest per riferimenti futuri (opzionale)
                    docker tag ${FULL_IMAGE_NAME} ${IMAGE_NAME}:latest
                '''
                
                echo "Container avviato con successo."
            }
        }
        
        stage('Cleanup Old Images') {
            steps {
                // Pulizia opzionale delle vecchie immagini
                sh '''
                    # Mantieni solo le ultime 5 immagini di questo progetto
                    docker images "${IMAGE_NAME}" --format "{{.ID}} {{.Tag}}" | 
                    sort -k2 -n | 
                    head -n -5 | 
                    awk '{print $1}' | 
                    xargs -r docker rmi || true
                    
                    # Rimuovi immagini dangling (non taggate)
                    docker image prune -f
                '''
            }
        }
    }
    
    post {
        success {
            echo "Deployment completato con successo"
            
            // Ottieni e visualizza informazioni sul container
            sh '''
                echo "Dettagli del container:"
                docker ps --filter "name=${CONTAINER_NAME}" --format "ID: {{.ID}}\\nNome: {{.Names}}\\nImage: {{.Image}}\\nPorte: {{.Ports}}\\nStatus: {{.Status}}\\n"
            '''
        }
        failure {
            echo "Deployment fallito"
        }
        always {
            // Notifica di completamento (opzionale)
            echo "Pipeline completata per il branch ${params.GIT_BRANCH}, build #${BUILD_NUMBER}"
        }
    }
}