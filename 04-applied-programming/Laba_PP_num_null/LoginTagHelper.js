export default class SAID {
  constructor(ifless, ifequal) {
    this.channel = new BroadcastChannel('SAID');
    this.getSAID = () => sessionStorage.SAID ?
        sessionStorage.SAID :
        (sessionStorage.SAID = (new Date()).getTime());
    
    this.postSAID = (data = {}) => {
      const message = {
        'SAID': this.getSAID(),
        'timestamp': Date.now(),
        ...data
      };
      this.channel.postMessage(message);
    };
    
    this.close = () => {
      this.channel.close();
    };
    
    this.ifless = ifless || (() => true);
    this.ifequal = ifequal || (() => true);
    
    this.channel.onmessage = (event) => {
      const receivedSAID = event.data.SAID;
      const currentSAID = this.getSAID();
      
      if (event.data.type === 'SYNC_STATE') {
        this.syncState(event.data.state, event.data.userName);
        return;
      }
      
      if (receivedSAID < currentSAID) {
        this.ifless();
      } else if (receivedSAID == currentSAID) {
        this.ifequal();
      } else {
        sessionStorage.SAID = receivedSAID;
        this.postSAID({type: 'REQUEST_SYNC'});
      }
    };
    
    setTimeout(() => {
      this.postSAID({type: 'REQUEST_SYNC'});
    }, 100);
  }
  
  syncState(state, userName) {
    if (window.syncAuthState) {
      window.syncAuthState(state, userName);
    }
  }
  
  broadcastState(state, userName) {
    this.postSAID({
      type: 'SYNC_STATE',
      state: state,
      userName: userName
    });
  }
}

export class SA {
  constructor(startState) {
    this.state = (SA.START[startState] != undefined) ? startState : SA.START.A;
    this.subscribers = {};
    this.on = (state, callback) => {
      if (!this.subscribers[state]) {
        this.subscribers[state] = [];
      }
      this.subscribers[state].push(callback);
    };
    this.emit = (state, data) => {
      if (this.subscribers[state])
        this.subscribers[state].forEach(callback => callback(data));
    };
    this.RunCommand = (command) => {
      let s = SA.TRANFUNC[this.state][command];
      console.log(this.state, ':RunCommand: command =', command, '--->', s);
      if (s != undefined) {
        this.state = s;
        console.log('RunCommand: new state =', this.state);
        this.emit(this.state, SA.TRANFUNC[this.state]);
      }
      return {[this.state]: SA.TRANFUNC[this.state]};
    };

    this.setState = (newState) => {
      this.state = newState;
      this.emit(this.state, SA.TRANFUNC[this.state]);
    };
  
  };
  static START = {
    A: 'A',
    N: 'N',
  };
  static COMMAND = {
    REG: 'REG', // registration
    SIN: 'SIN', // sign in
    CAN: 'CAN', //cancel
    ADE: 'ADE', // access denied
    LIN: 'LIN', //logged in
    SOU: 'SOU', //sign out
    REJ: 'REJ' //reg rej
  };
  static STATES =
  {
      A: SA.START.A, 
      N: SA.START.N, 
      S: 'S', //get sign in  
      R: 'R', //get reg
      SV: 'SV', //validation s
      RV: 'RV' // validation r
  };
  static TRANFUNC = {
    [SA.STATES.A]: {[SA.COMMAND.SOU]: SA.STATES.N},
    [SA.STATES.N]:
        {[SA.COMMAND.SIN]: SA.STATES.S, [SA.COMMAND.REG]: SA.STATES.R},
    [SA.STATES.S]:
        {[SA.COMMAND.SIN]: SA.STATES.SV, [SA.COMMAND.CAN]: SA.STATES.N},
    [SA.STATES.R]:
        {[SA.COMMAND.REG]: SA.STATES.RV, [SA.COMMAND.CAN]: SA.STATES.N},
    [SA.STATES.SV]:
        {[SA.COMMAND.LIN]: SA.STATES.A, [SA.COMMAND.ADE]: SA.STATES.S},
    [SA.STATES.RV]:
        {[SA.COMMAND.SIN]: SA.STATES.S, [SA.COMMAND.REJ]: SA.STATES.R}
  };
};

export async function Registration(name, pass, sin, rej) {
  const users = getUsers();

  if (users[name]) {
    rej();
    return false;
  }

  users[name] = {password: pass};
  const success = saveUsers(users);

  if (success) {
    sin();
    return true;
  } else {
    rej();
    return false;
  }
};

export async function SignIn(name, pass, lin, ade) {
  const users = getUsers();

  if (users[name] && users[name].password === pass) {
    const success = setAuthState(true, name);

    if (success) {
      lin();
      return true;
    } else {
      ade();
      return false;
    }
  } else {
    ade();
    return false;
  }
};

export async function SignOut(sou) {
  const success = clearAuthState();
  sou();
  return success;
};

export async function GetUserName(setA, setN) {
  try {
    const isLoggedIn =
        localStorage.getItem(STORAGE_KEYS.IS_LOGGED_IN) === 'true';
    const userName = localStorage.getItem(STORAGE_KEYS.USER_NAME);

    if (isLoggedIn && userName) {
      setA(userName);
      return true;
    } else {
      setN();
      return false;
    }
  } catch (error) {
    setN();
    return false;
  }
};

export function getSAID() {
  sessionStorage.SAID ? sessionStorage.SAID :
                        (sessionStorage.SAID = (new Date()).getTime());
};

export const STORAGE_KEYS = {
  USER_DATA: 'user_data',
  USER_NAME: 'user_name',
  IS_LOGGED_IN: 'is_logged_in'
};

function getUsers() {
  try {
    const users = localStorage.getItem(STORAGE_KEYS.USER_DATA);
    return users ? JSON.parse(users) : {};
  } catch (error) {
    console.error('Error reading users from localStorage:', error);
    return {};
  }
}

function saveUsers(users) {
  try {
    localStorage.setItem(STORAGE_KEYS.USER_DATA, JSON.stringify(users));
    return true;
  } catch (error) {
    console.error('Error saving users to localStorage:', error);
    return false;
  }
}

function setAuthState(isLoggedIn, userName = null) {
  try {
    localStorage.setItem(STORAGE_KEYS.IS_LOGGED_IN, isLoggedIn.toString());
    if (userName) {
      localStorage.setItem(STORAGE_KEYS.USER_NAME, userName);
    }
    return true;
  } catch (error) {
    console.error('Error setting auth state:', error);
    return false;
  }
}

function clearAuthState() {
  try {
    localStorage.removeItem(STORAGE_KEYS.IS_LOGGED_IN);
    return true;
  } catch (error) {
    console.error('Error clearing auth state:', error);
    return false;
  }
}