# Appserver

## Developing with Azure

To develop locally one will need to start the Azure Storage Emulator. Instructions can be found
here: [Azure Storage Emulator](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-emulator)

## Setting up Databases

Run these commands from the Package Manager Console to setup DBs:

``` 
Update-Database -Context SubmissionContext
Update-Database -Context SubmissionStagingContext
```

## Setting up Blob Storage

When developing locally, add the following environment variable:

```
BLOB_CONNECTION		UseDevelopmentStorage=true;
```

When deploying, navigate to Azure Portal, click on you storage account, click on "Access keys" on the left side, and add the following environment variable:

```
BLOB_CONNECTION		any of the available connection strings
```
