﻿public enum ServerPckts {
    WELCOME_MSG         = 1,
    SERVER_RESPONSE     = 2,
    GET_PLAYER_INFO     = 3,
    ADDED_TO_MATCH      = 4,
    PLAYER_JOINED_MATCH = 5
}

public enum ClientPckts {
    LOGIN               = 1,
    THANKS_WELCOME      = 2,
    REGISTER_ACCOUNT    = 3,
    QUIT                = 4,
    GET_PLAYER_INFO     = 5,
    APP_VERSION         = 6,
    SEARCH_MATCH        = 7
}
