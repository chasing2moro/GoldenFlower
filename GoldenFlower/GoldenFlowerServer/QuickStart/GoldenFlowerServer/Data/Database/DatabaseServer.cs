using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DatabaseServer : DatabaseRecord
{

    [DatabaseAttributeString]
    public string ip;

    [DatabaseAttributeInt]
    public int port;
}

