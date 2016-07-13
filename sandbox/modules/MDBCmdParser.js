//var _MDB = require('mdb').create();

var INTERNAL = {
    buffer: Uint8Array(36),
    bufLen: 0,
    result: null
}

var SUB_CMD_LENGTH = {
    SETUP:     [6, 6],
    VEND:      [6, 2, 4, 2, 2, 6],
    EXPANSION: [31],
    READER:    [2, 2, 2]
};

function MDBCmdParser() {
};

MDBCmdParser.prototype._appendCmdBytes = function(data) {
  for(var i=0; i<data.length; i++) {
    INTERNAL.buffer[INTERNAL.bufLen+i] = data[i];
  }
  INTERNAL.bufLen += data.length;  
}

MDBCmdParser.prototype._buildResult = function() {
    INTERNAL.result = new Uint8Array(INTERNAL.bufLen);
    for(var i=0; i<INTERNAL.bufLen; i++) {
        INTERNAL.result[i] = INTERNAL.buffer[i];
    }
    INTERNAL.bufLen = 0;
    //TODO: clear INTERNAL.buffer
}

MDBCmdParser.prototype.putData = function(data) {
  var cmd = -1;  //command 
  var sub = -1;  //subcommand
  
  this._appendCmdBytes(data);
  if(INTERNAL.bufLen > 0) {
    cmd = INTERNAL.buffer[0] & _MDB.MASK.COMMAND
  }
  if (INTERNAL.bufLen > 1) {
    sub = INTERNAL.buffer[1];
  }
  // start command processing
  switch(cmd) {
    case 0x00://_MDB.CASHLESS_MSG.RESET:
        // NO SUBCOMMAND
        if(INTERNAL.bufLen > 1) {
          this._buildResult();
        }
        break;
    case 0x01://_MDB.CASHLESS_MSG.SETUP:
        // Command length must equal: LEN(CMD) + LEN(CHK)        
        if(INTERNAL.bufLen > SUB_CMD_LENGTH.SETUP[sub]) {
            this._buildResult();
        }
        break;
    case 0x02://_MDB.CASHLESS_MSG.POLL:
        if(INTERNAL.bufLen > 1) {
        this._buildResult();
      }
      break;
    case 0x03://_MDB.CASHLESS_MSG.VEND:
        // Command length must equal: LEN(CMD) + LEN(CHK)
        if(INTERNAL.bufLen > SUB_CMD_LENGTH.VEND[sub]) {
            this._buildResult();
        }
        break;
    case 0x04://_MDB.CASHLESS_MSG.READER:
        // Command length must equal: LEN(CMD) + LEN(CHK)
        if(INTERNAL.bufLen > SUB_CMD_LENGTH.READER[sub]) {
            this._buildResult();
        }
        break;
    case 0x07://_MDB.CASHLESS_MSG.EXPANSION:
        if(INTERNAL.bufLen > SUB_CMD_LENGTH.EXPANSION[sub]) {
            this._buildResult();
        }
        break;        
    }
}

MDBCmdParser.prototype.getResult = function() {
    return INTERNAL.result;
}

MDBCmdParser.prototype.clearResult = function() {
    INTERNAL.result = null;
}

//
// Export method for create object
exports.create = function() {
    return new MDBCmdParser();
}