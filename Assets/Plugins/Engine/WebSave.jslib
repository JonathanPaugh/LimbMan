var webSave = {
	$Save: {
		database: undefined,
		profile: undefined,
		saveData: undefined,
	},

	$WebSaveConnect: function(callback) {
		request = window.indexedDB.open("jape", 1);

		request.onerror = function(event) {
			console.log("IndexedDB could not be loaded");
		};

		request.onsuccess = function(event) {
			Save.database = event.target.result;
			console.log("IndexedDB loaded");
			callback();
		};

		request.onupgradeneeded = function(event) {
			Save.database = event.target.result;
			store = Save.database.createObjectStore("save", { keyPath: "profile" });
			console.log("IndexedDB created or updated");
		};
	},

	$WebSaveStore: function() {
		transaction = Save.database.transaction(["save"], "readwrite");

    	transaction.onerror = function(event) {
			console.error(event);
		};

		transaction.oncomplete = function(event) {
			console.log("Transaction Complete");
		};

    	return transaction.objectStore("save");
	},

	$WebSaveGet: function(callback) {
    	store = WebSaveStore();
    	request = store.get(Save.profile);

		request.onerror = function(event) {
    		console.error(event);
    	};

    	request.onsuccess = function(event) {
    		console.log(event);

    		result = request.result;

    		if (result) {
    			console.log("Game Loaded");
    			callback(result.data);
    		} else {
    			console.log("Unable to get load data");
    			callback(null);
    		}
    	};
    },

    $WebSaveSet: function(callback) {
    	store = WebSaveStore();
    	request = store.put({ profile: Save.profile, data: Save.saveData });

		request.onerror = function(event) {
    		console.error(event);
            callback(null);
    	};

    	request.onsuccess = function(event) {
    		console.log("Game Saved");
            callback(null);
    	};
    },

    $WebSaveDelete: function(callback) {
    	store = WebSaveStore();
    	request = store.delete(Save.profile);

		request.onerror = function(event) {
    		console.error(event);
            callback(null);
    	};

    	request.onsuccess = function(event) {
    		console.log("Game Deleted");
            callback(null);
    	};
    },

    WebSaveRequestSave: function(profile, data, length) {
    	Save.profile = profile;
	    Save.saveData = new Uint8Array(length);
	    for (var i = 0; i < length; i++) {
	    	Save.saveData[i] = HEAP8[data + i];
	    }

        if (Save.database == null) { 
        	WebSaveConnect(function() {
                WebSaveSet(WebSaveReceive)
            });
        }
        else 
        {
        	WebSaveSet(WebSaveReceive);
        }
    },

    WebSaveRequestLoad: function(profile) {
    	Save.profile = profile;

    	if (Save.database == null) { 
        	WebSaveConnect(function() {
				WebSaveGet(WebSaveReceive)
        	});
        }
        else 
        {
        	WebSaveGet(WebSaveReceive);
        }
    },

    WebSaveRequestDelete: function(profile) {
    	Save.profile = profile;

		if (Save.database == null) { 
        	WebSaveConnect(function() {
				WebSaveDelete(WebSaveReceive)
        	});
        }
        else 
        {
        	WebSaveDelete(WebSaveReceive);
        }
    },

    $WebSaveReceive: function(data) {
    	if (data == null) {
            SendMessage("WebManager", "WebSaveReceiveOpen", 0);
			SendMessage("WebManager", "WebSaveReceiveClose");
    	} else {
    		SendMessage("WebManager", "WebSaveReceiveOpen", data.byteLength);
		    buffer = _malloc(data.byteLength);
		    heap = new Uint8Array(HEAPU8.buffer, buffer, data.byteLength);
		    heap.set(new Uint8Array(data));
			SendMessage("WebManager", "WebSaveReceiveData", buffer);
            SendMessage("WebManager", "WebSaveReceiveClose");
    	}
    },
};

autoAddDeps(webSave, "$Save");
autoAddDeps(webSave, "$WebSaveConnect");
autoAddDeps(webSave, "$WebSaveStore");
autoAddDeps(webSave, "$WebSaveSet");
autoAddDeps(webSave, "$WebSaveGet");
autoAddDeps(webSave, "$WebSaveDelete");
autoAddDeps(webSave, "$WebSaveReceive");

mergeInto(LibraryManager.library, webSave);