public enum Response { 
    NONE_RESPONSE                       = -1,
    ERROR                               =  0,
    OK                                  =  1,
    REGISTER_USER_DUPLICATED_ERROR      =  2,
    LOGIN_WRONG_USER_OR_PASSWORD_ERROR  =  3,
    WORKING_OFFLINE                     =  4,
    EXPIRATION_TIME_ERROR               =  5,
    UNMATCH_APP_VERSION_ERROR           =  6,
    PLAYER_ALREADY_ONLINE_ERROR         =  7,
    ADDED_TO_MATCH_QUEUE                =  8,
    LOAD_MATCH_SCENE                    =  9,
    PLAYER_JOINED_MATCH                 = 10,
    RECEIVED_CARD_LIST                  = 11,
    RECEIVED_CARDS_PER_PLAYER_AND_TURN  = 12,
    ADDED_TO_MATCH                      = 13,
    TEST_GAME_UPDATE                    = 14
}
