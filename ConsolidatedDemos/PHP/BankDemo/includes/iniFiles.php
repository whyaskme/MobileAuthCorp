<?php
// function for reading "ini" style config files

function read_ini_file($filename) {
    if(file_exists($filename)) {
        $ini_array = parse_ini_file($filename, true); // handles file in the style of the "php.ini" file
        return $ini_array;
    }
    else {
        return false;
    }
}

function write_ini_file($array, $file) {
    $res = array();
    foreach($array as $key => $val) {
        if(is_array($val)) {
            $res[] = "[$key]";
            foreach($val as $skey => $sval) $res[] = "$skey = ".(is_numeric($sval) ? $sval : '"'.$sval.'"');
        }
        else $res[] = "$key = ".(is_numeric($val) ? $val : '"'.$val.'"');
    }
    safefilerewrite($file, implode("\r\n", $res));
}

function safefilerewrite($fileName, $dataToSave) {   
    if ($fp = fopen($fileName, 'w')) {
        $startTime = microtime();
        do { 
           $canWrite = flock($fp, LOCK_EX);
           // If lock not obtained sleep for 0 - 100 milliseconds, to avoid collision and CPU load
           if(!$canWrite) usleep(round(rand(0, 100)*1000));
        } while ((!$canWrite)and((microtime()-$startTime) < 1000));

        //file was locked so now we can store information
        if ($canWrite) {
            fwrite($fp, $dataToSave);
            flock($fp, LOCK_UN);
        }
        fclose($fp);
    }

}

?>
