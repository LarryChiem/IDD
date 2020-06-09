# Appserver

## Developing with Azure

To develop locally one will need to start the Azure Storage Emulator. Instructions can be found
here: [Azure Storage Emulator](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-emulator)

## Setting up Databases

Due to a particular migration for synchronizing the `Submission` and `Staging` `Id` it is required that one
first setup the Appserver database below before setting up the AdminUI database.

The Appserver itself will run migrations upon startup, or can be done via the Package Manager Console in
Visual Studios following the instructions below.

### Updating Appserver Database

Run these commands from the Package Manager Console to setup the `SubmissionStagingContext` DBs:

``` 
Update-Database -Context SubmissionStagingContext
```

## Setting up Blob Storage

When developing locally, add the following environment variable:

```
BLOB_CONNECTION		UseDevelopmentStorage=true;
```

When deploying, navigate to Azure Portal, click on you storage account, click on "Access keys" on the 
left side, and add the following environment variable:

```
BLOB_CONNECTION		any of the available connection strings
```

## Working with Amazon S3

In order to interface with S3 buckets, add the following environment variable:

```
BUCKET_NAME         some string
```
