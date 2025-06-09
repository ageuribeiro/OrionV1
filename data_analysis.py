''' Importing Librarys'''

import time
import pandas as pd
import requests


# Carregando o dataset
df = pd.read_csv("Datasets/dataset_customer_names.csv")

# Obtendo todos os dados do arquivo CSV
firstnames = df["First Name"]
lastnames = df["Last Name"]

# Criar produto cartesiano (todas as combinações possiveis)
full_df = pd.DataFrame([(f, l) for f in firstnames for l in lastnames], columns=[
                       "firstname", "lastname"])

# Criar coluna de nome completo no dataframe
full_df["fullname"] = full_df["firstname"]+" " + full_df["lastname"]

# Obtendo nomes únicos para reduzir as chamadas
unique_names = df["First Name"].unique()

# Criando dicionário para armazenar os gêneros
gender_map = {}

# Consultando a API
for name in unique_names:
    try:
        response = requests.get(f"https://api.genderize.io?name={name}", timeout=5)
        data = response.json()
        gender = data.get("gender")
        gender_map[name] = gender.capitalize() if gender else "Indefinido"
        time.sleep(1)
    except requests.exceptions.RequestException as e:
        gender_map[name] = "Indefinido"
        print(f"Erro com o nome {name}: {e}")

# Adicionando a nova coluna gender ao dataframe

df["gender"] = df["firstnames"].map(gender_map)

# Salvar no arquivo CSV
full_df.to_csv("Datasets/todas_combinacoes.csv", index=False)


print(df.head())
