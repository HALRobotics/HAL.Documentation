# HAL.Benchmarking
This repository contains tools to enable the benchmarking of different compute hardware within our stack. The main use of this is a docker container which can be run and will export results into a local folder.

N.B. This should ony ever be run on a PC when it's the only thing running and might take a few hours to run.

### Setup

1. Install [docker](https://www.docker.com/)
1. Install the [Azure CLI](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli)
1. Log in to the CLI - `az login --use-device-code`
1. Add our docker registry to docker - `az acr login --name halwebuserregistry.azurecr.io`
1. Pull the image - `docker pull halwebuserregistry.azurecr.io/halbenchmarking:latest`
1. Run the image replacing "{/path/to/results}" with the path you want to store the results in and "{MYPCNAME}" - `docker run --rm -it -v "{/path/to/results}:/results" -e ALIAS={MYPCNAME} halwebuserregistry.azurecr.io/halbenchmarking`
1. Wait for the container to complete
1. Upload the results to this repository