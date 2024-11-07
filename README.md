# RabbitMQ-api

# Step 1 (change the host file)
Update host file on your PC like this instruction https://www.nublue.co.uk/guides/edit-hosts-file/#:~:text=In%20Windows%2010%20the%20hosts,%5CDrivers%5Cetc%5Chosts.

Need to path these lines

127.0.0.1 www.test.com
0.0.0.0 www.test.com
192.168.0.5 www.test.com

192.168.0.133 host.docker.internal

# Step 2 (start the app)

1. Select a project in the terminal
2. You need to put this command: docker-compose up
