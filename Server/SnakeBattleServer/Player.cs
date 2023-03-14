using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace SnakeBattleServer
{

    internal class Player
    {
        // Make thes getters setters instead of public

        public int room;
        public IPEndPoint ip;
        public playerStates playerState;

        public Player(int room, IPEndPoint iPEndPoint)
        {
            this.room = room;
            this.ip = iPEndPoint;
        }
    }

    enum playerStates
    {
        WIN,
        LOSE,
        TIE
    }
}
