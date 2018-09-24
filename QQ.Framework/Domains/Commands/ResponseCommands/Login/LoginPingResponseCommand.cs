﻿using QQ.Framework.Events;
using QQ.Framework.Packets.Receive.Login;
using QQ.Framework.Packets.Send.Login;
using QQ.Framework.Utils;

namespace QQ.Framework.Domains.Commands.ResponseCommands.Login
{
    [ResponsePacketCommand(QQCommand.Login0x0825)]
    public class LoginPingResponseCommand : ResponseCommand<Receive_0x0825>
    {
        public LoginPingResponseCommand(QQEventArgs<Receive_0x0825> args) : base(args)
        {
        }

        public override void Process()
        {
            if (_packet.Result == 0xFE)
            {
                _service.MessageLog($"服务器{ _user.TXProtocol.dwRedirectIP}重定向");
                //如果是登陆重定向，继续登陆
                _user.IsLoginRedirect = true;
                //刷新重定向后服务器IP
                _service.RefreshHost(_user.TXProtocol.dwRedirectIP);
                //累加重定向记录
                _user.TXProtocol.wRedirectCount += 1;
                _user.TXProtocol.RedirectIP.Add(Util.IPStringToByteArray(_user.TXProtocol.dwRedirectIP));
                //重新发送登录Ping包
                _service.Send(new Send_0x0825(_user, true));
            }
            else
            {
                _service.MessageLog($"连接服务器{_user.TXProtocol.dwServerIP}成功");
                //Ping请求成功后发送0836登录包
                _service.Send(new Send_0x0836(_user, Login0x0836Type.Login0x0836_622));
            }
        }
    }
}