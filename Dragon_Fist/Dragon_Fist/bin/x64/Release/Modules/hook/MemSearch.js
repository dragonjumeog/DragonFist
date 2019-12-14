function memscan(pattern) {
    var ranges = Process.enumerateRangesSync({protection: 'rw-', });
    var range;
    function processNext(){
		range = ranges.pop();
       if(!range){
           send('Memory Scan Finished!');
           return;
       }
       Memory.scan(range.base, range.size, pattern, {
	        onMatch: function(address, size){
               send('[+] Pattern found at: ' + address.toString());
           }, 
           onError: function(reason){
               ;
           }, 
           onComplete: function(){
               processNext();
           }
       });
   }
   processNext();
}
Java.perform(function(){
    memscan('D6 06 ');
});
