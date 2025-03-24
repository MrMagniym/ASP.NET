FROM node:22.12-alpine
WORKDIR /app
COPY package.json ./
RUN npm install
RUN npm install -g dotenv-cli
COPY . .
CMD ["npm", "run", "docker"]