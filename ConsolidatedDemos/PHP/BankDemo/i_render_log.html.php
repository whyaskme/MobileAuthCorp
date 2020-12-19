<div id="columnMain" style="margin-bottom:0 !important;">
    <div id="main-content" class="container">
        <p class="page-header">MAC Bank Transaction Log</p>
        <div id="transaction_log">
            <?php
                if(!empty($transLog)) {
                    echo '<h4>Transactions:</h4>';
                    echo '<div style="margin:0 !important;padding:0 !important;border:1px solid #b3b3b3 !important;">';
                    echo '<table id="cart-content" style="z-index:0;">
                        <thead>
                            <tr>
                                <th>Date</th><th>Transaction</th>
                            </tr>
                        </thead>
                        <tbody>';
		            foreach($transLog as $log) {
                        echo '<tr>
                                <td>
                                    <div class="visible-sm visible-xs" style="margin-top:0.5rem;"><strong>Primary Account</strong></div>
                                    '.$log['Date'].'
                                </td>
                                <td style="word-wrap: break-word !important;white-space: normal !important;">
                                    <div class="visible-sm visible-xs" style="margin-top:0.5rem;"><strong>Balance</strong></div>
                                    '.str_replace('|', ' ', $log['Transaction']).' '.str_replace('|',' ', $log['Items']).'
                                </td>
                              </tr>';
		            }
                    echo '</tbody>';
                    echo '</table>';
                    echo '</div>';
	            }
             ?>
        </div> <!-- end transaction log -->
    </div>    
</div>
<script type="text/javascript">
    $(document).ready(function () {
        $("#footer_txt").hide();
        $("#menu_log").addClass("selected");
    });
</script>