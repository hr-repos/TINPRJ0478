# nodes

<b>a node must have 2 at least these 2 variables to be able to communicate with the Mqtt part of the communication:</b>
 - \<id> example: <i>{"id": "vkl1_ID", "value": 1, "writeable": false}</i>
 - \<type> example: <i>{"id": "vkl1_type", "value": "vkl", "writeable": false}</i>


<b>after that you can add what ever variables you want but we added the *'mode'* variable to send the message data from mqtt to opcua.
you can see the opcua server as just a pipeline te transfer the mqtt message from 1 apo to the other so for more information go to mqtt protocol.</b>