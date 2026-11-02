# Il container "toolbox": la CLI fga e gli strumenti che servono ai comandi dell'articolo.
FROM alpine:3.20

RUN apk add --no-cache bash curl openssl jq make

# La CLI ufficiale, copiata dalla sua immagine (che e' senza shell).
COPY --from=openfga/cli:v0.7.19 /fga /usr/local/bin/fga-bin
COPY fga-wrapper.sh /usr/local/bin/fga
COPY profile.sh /etc/toolbox-profile.sh
RUN chmod +x /usr/local/bin/fga

# Le shell interattive leggono questo file: FGA_STORE_ID e' subito disponibile.
ENV ENV=/etc/toolbox-profile.sh
WORKDIR /work
