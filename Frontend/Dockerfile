FROM node:14.15.3-alpine3.10 as build
EXPOSE 49153
EXPOSE 80
WORKDIR /app

RUN npm install -g @angular/cli

COPY ./package.json .
RUN npm install
COPY . .
RUN ng build --prod

FROM nginx as runtime
COPY --from=build /app/dist /usr/share/nginx/html
