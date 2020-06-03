FROM microsoft/dotnet:2.1-runtime
#create logs folder
RUN mkdir /var/log/netcoreapp
RUN chmod 777 /var/log/netcoreapp
#create the application folder in /home
RUN mkdir /home/app
ADD PublishOutput /home/app/
#set the working directory for ENTRYPOINT
WORKDIR /home/app
ENTRYPOINT ["dotnet", "OneDriveLinkCreator.dll"]