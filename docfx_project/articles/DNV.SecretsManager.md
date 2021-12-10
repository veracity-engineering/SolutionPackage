# Secrets Manager

DNV.SecretsManager is a command line tool for managing secrets from the Azure KeyVault or Azure DevOps Variable Groups.

This tool allows collections of secrets to be downloaded and uploaded as structured JSON files.

## Example
Given a large collection of key vault secrets in an Azure Key vault such as those shown below:
![KeyVault secrets](../images/DNV.SecretsManager/keyvault-secrets.png)

The secrets manager tool could be executed with a command:

```
secretsmanager keyvault -d -s <keyvault-url> -f output-file.json
```

The resulting `output-file.json` would look like:

```json
{
  "Account": {
    "BaseUrl": <secret value>
  },
  "Company": {
    "ApiKey": <secret value>,
    "Authority": <secret value>,
    "BaseUrl": <secret value>,
    "ClientId": <secret value>,
    "ClientSecret": <secret value>,
    "Resource": <secret value>
  },
  "Customer": {
    "ApiKey": <secret value>,
    "Authority": <secret value>,
    "BaseUrl": <secret value>,
    "ClientId": <secret value>,
    "ClientSecret": <secret value>,
    "IsApiKeyOnly": <secret value>,
    "Resource": <secret value>
  },
  "Emailer": {
    "BaseUri": <secret value>,
    "FunctionKey": <secret value>
  ...
```

Conversely, an input json file (`input-file.json`) could be uploaded to an Azure Key vault by executing a command:

```
secretsmanager keyvault -u -s <keyvault-url> -f input-file.json
```

---
# Useage

```
secretsmanager   <command> [<args>]

Commands:
        keyvault        Download or upload secrets from/to Azure Keyvault
        variablegroup   Download or upload secrets from/to Azure Keyvault
```

---

## Key vault command

```
secretsmanager keyvault			[-h | --help]
						 		-d | --download -u | --upload
						 		-s | --url <url>
						 		-f | --filename <filename>
```

### Options
`-h | --help`

Prints the synopsis of commands and options available.

`-d | --download`

Requests the secrets to be downloaded from the specified source to a JSON file.

`-u | --upload`

Requests that a provided JSON file be uploaded to a specified source.

`-s | --url <url>`

Provide the URL to the keyvault.

`-f | --filename <filename>`

Specify the file to which you would like to download to or upload from.

---

## Variable group command
```
secretsmanager variablegroup	[-h | --help]
								-d | --download -u | --upload
								-s | --base-url <base-url>
								-o | --organization <organization>
								-p | --pat <pat>
								-g | --group-id <group-id>
								-f | --filename <filename>
```
### Options
`-h | --help`

Prints the synopsis of commands and options available.

`-d | --download`

Requests the secrets to be downloaded from the specified source to a JSON file.

`-u | --upload`

Requests that a provided JSON file be uploaded to a specified source.

`-s | --base-url <base-url>`

Provide the base URL to the Azure DevOps.

`-o | --organization <organization>`

Provide the organization under Azure DevOps to which a variable group belongs.

`-p | --pat <pat>`

Specify the Person Access Token for authentication.

`-g | --group-id <group-id>`

Specify the id of the variable group you would like to download from or upload to.

`-f | --filename <filename>`

Specify the file to which you would like to download to or upload from.