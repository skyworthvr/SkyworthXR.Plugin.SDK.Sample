using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IState
{
    void OnStateEnter(object data);
    void OnStateExit(object data);

    void OnStateBreathe();
}
