public enum ServerPckts {
    WELCOME_MSG                         = 1,
    SERVER_RESPONSE                     = 2,
    GET_PLAYER_INFO                     = 3,
    ADDED_TO_MATCH                      = 4,
    PLAYER_JOINED_MATCH                 = 5,
    MATCH_STARTS                        = 6,
    SENDING_CARD                        = 7,
    SENDING_LIST_CARDS                  = 8,
    CARDS_PER_PLAYER_AND_INITIAL_TURN   = 9,
    TEST_GAME_UPDATE                    = 10,
    HAND_CARDS_UPDATE                   = 11
}

public enum ClientPckts {
    LOGIN               = 1,
    THANKS_WELCOME      = 2,
    REGISTER_ACCOUNT    = 3,
    QUIT                = 4,
    GET_PLAYER_INFO     = 5,
    APP_VERSION         = 6,
    SEARCH_MATCH        = 7,
    TEST_GAME_MOVEMENT  = 8,
    ASK_FOR_CARDS       = 9,
    HAND_CARDS_UPDATE   = 10,
    PASS_TURN           = 11
}
